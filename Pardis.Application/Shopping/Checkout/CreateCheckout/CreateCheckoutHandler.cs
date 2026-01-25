using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Payments.Contracts;
using Pardis.Domain.Shopping;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;
using Pardis.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace Pardis.Application.Shopping.Checkout.CreateCheckout;

public class CreateCheckoutHandler : IRequestHandler<CreateCheckoutCommand, OperationResult<CreateCheckoutResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IRepository<UserCourse> _userCourseRepository;
    private readonly IMapper _mapper;

    public CreateCheckoutHandler(
        ICartRepository cartRepository,
        IOrderRepository orderRepository,
        IEnrollmentRepository enrollmentRepository,
        IRepository<UserCourse> userCourseRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _enrollmentRepository = enrollmentRepository;
        _userCourseRepository = userCourseRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CreateCheckoutResult>> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // کل عملیات اتمیک و Retry-safe
            var op = await _orderRepository.ExecuteInTransactionAsync<OperationResult<CreateCheckoutResult>>(async ct =>
            {
                // بررسی IdempotencyKey
                if (!string.IsNullOrEmpty(request.IdempotencyKey))
                {
                    var existingByIdempotency = await _orderRepository.Table
                        .FirstOrDefaultAsync(o => o.UserId == request.UserId && o.IdempotencyKey == request.IdempotencyKey, ct);

                    if (existingByIdempotency != null)
                    {
                        var lastAttempt = existingByIdempotency.PaymentAttempts.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                        return OperationResult<CreateCheckoutResult>.Success(MapToResult(existingByIdempotency, lastAttempt, "سفارش قبلاً ایجاد شده است (Idempotency)"));
                    }
                }

                // دریافت سبد خرید کاربر
                var cart = await _cartRepository.GetByUserIdAsync(request.UserId, ct);
                if (cart == null || cart.IsEmpty())
                    return OperationResult<CreateCheckoutResult>.Error("سبد خرید خالی است");

                if (cart.IsExpired())
                    cart.ExtendExpiry();

                // CRITICAL: Validate CartId is not empty before creating order
                if (cart.Id == Guid.Empty)
                    return OperationResult<CreateCheckoutResult>.Error("شناسه سبد خرید معتبر نیست");

                // بررسی وجود سفارش پرداخت نشده برای همین سبد
                var existingOrderForCart = await _orderRepository.Table
                    .Include(o => o.PaymentAttempts)
                    .FirstOrDefaultAsync(o => o.UserId == request.UserId && 
                                              o.CartId == cart.Id && 
                                              (o.Status == OrderStatus.PendingPayment || o.Status == OrderStatus.Draft), ct);

                if (existingOrderForCart != null)
                {
                    var lastAttempt = existingOrderForCart.PaymentAttempts.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                    return OperationResult<CreateCheckoutResult>.Success(MapToResult(existingOrderForCart, lastAttempt, "شما یک سفارش پرداخت نشده برای این سبد دارید"));
                }

                // بررسی ثبت‌نام قبلی
                foreach (var item in cart.Items)
                {
                    var existingEnrollment =
                        await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, item.CourseId, ct);

                    if (existingEnrollment != null)
                        return OperationResult<CreateCheckoutResult>.Error(
                            $"شما قبلاً در دوره '{item.TitleSnapshot}' ثبت‌نام کرده‌اید");
                }

                // ایجاد سفارش
                var order = new Order(request.UserId, cart, request.IdempotencyKey);

                // مبلغ پرداخت
                var paymentAmount = cart.TotalAmount;
                var isFreePurchase = paymentAmount == 0;

                // تنها روش پرداخت مجاز: کارت به کارت (Manual)
                // دوره‌های رایگان بلافاصله تکمیل می‌شوند
                if (isFreePurchase)
                {
                    // برای دوره‌های رایگان، بلافاصله ثبت‌نام ایجاد می‌کنیم
                    order.CompleteOrder();
                    await CreateEnrollmentsForFreeCoursesAsync(request.UserId, cart, ct);
                    cart.Clear();
                    
                    return OperationResult<CreateCheckoutResult>.Success(MapToResult(order, null, "خرید رایگان با موفقیت تکمیل شد"));
                }

                // ایجاد تلاش پرداخت دستی
                var paymentAttempt = new PaymentAttempt(order.Id, request.UserId, paymentAmount);
                paymentAttempt.StartPayment();
                
                order.PaymentAttempts.Add(paymentAttempt);

                // ذخیره سفارش
                await _orderRepository.CreateAsync(order, ct);

                return OperationResult<CreateCheckoutResult>.Success(MapToResult(order, paymentAttempt));
            }, cancellationToken);

            return op;
        }
        catch (Exception ex)
        {
            return OperationResult<CreateCheckoutResult>.Error($"خطا در ایجاد سفارش: {ex.Message}");
        }
    }

    private CreateCheckoutResult MapToResult(Order order, PaymentAttempt? attempt, string? message = null)
    {
        var isFree = order.TotalAmount == 0;
        return new CreateCheckoutResult
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            PaymentAttemptId = attempt?.Id ?? Guid.Empty,
            TrackingCode = attempt?.TrackingCode ?? string.Empty,
            TotalAmount = order.TotalAmount,
            PaymentMethod = PaymentMethod.Manual, // تنها روش پرداخت مجاز
            PaymentStatus = attempt?.Status ?? PaymentAttemptStatus.Draft,
            RequiresReceiptUpload = attempt?.RequiresReceiptUpload() ?? false,
            IsFreePurchase = isFree,
            Message = message ?? (isFree ? "خرید رایگان با موفقیت تکمیل شد" : "سفارش با موفقیت ایجاد شد")
        };
    }

    private async Task CreateEnrollmentsForFreeCoursesAsync(string userId, Domain.Shopping.Cart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            var enrollment = new CourseEnrollment(item.CourseId, userId, 0);
            enrollment.AddPayment(0, "FREE_PURCHASE", EnrollmentPaymentMethod.Transfer);

            await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);

            var alreadyInLms = await _userCourseRepository.AnyAsync(
                uc => uc.UserId == userId && uc.CourseId == item.CourseId,
                cancellationToken);

            if (!alreadyInLms)
            {
                var userCourse = new UserCourse
                {
                    UserId = userId,
                    CourseId = item.CourseId,
                    EnrolledAt = DateTime.UtcNow,
                    PurchasePrice = item.UnitPrice,
                    Status = StudentCourseStatus.Active
                };

                await _userCourseRepository.AddAsync(userCourse);
            }
        }
    }
}
