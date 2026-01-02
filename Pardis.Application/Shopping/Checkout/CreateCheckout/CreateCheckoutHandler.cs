using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Payments.Contracts;
using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.Checkout.CreateCheckout;

/// <summary>
/// پردازشگر دستور ایجاد checkout
/// </summary>
public class CreateCheckoutHandler : IRequestHandler<CreateCheckoutCommand, OperationResult<CreateCheckoutResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public CreateCheckoutHandler(
        ICartRepository cartRepository,
        IOrderRepository orderRepository,
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CreateCheckoutResult>> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت سبد خرید کاربر
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null || cart.IsEmpty())
                return OperationResult<CreateCheckoutResult>.Error("سبد خرید خالی است");

            // بررسی انقضای سبد
            if (cart.IsExpired())
                return OperationResult<CreateCheckoutResult>.Error("سبد خرید منقضی شده است");

            // بررسی اینکه کاربر قبلاً در دوره‌های موجود در سبد ثبت‌نام نکرده باشد
            foreach (var item in cart.Items)
            {
                var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, item.CourseId, cancellationToken);
                if (existingEnrollment != null)
                    return OperationResult<CreateCheckoutResult>.Error($"شما قبلاً در دوره '{item.TitleSnapshot}' ثبت‌نام کرده‌اید");
            }

            // ایجاد سفارش
            var order = new Order(request.UserId, cart);
            order = await _orderRepository.CreateAsync(order, cancellationToken);

            // تعیین مبلغ پرداخت
            var paymentAmount = cart.TotalAmount;
            var isFreePurchase = paymentAmount == 0;

            // اگر خرید رایگان است، روش پرداخت را Free تنظیم کن
            if (isFreePurchase)
                request.PaymentMethod = PaymentMethod.Free;

            // ایجاد تلاش پرداخت
            var paymentAttempt = order.CreatePaymentAttempt(request.PaymentMethod, paymentAmount);

            // اگر خرید رایگان است، مستقیماً تکمیل کن
            if (isFreePurchase)
            {
                paymentAttempt.MarkAsPaid("FREE_PURCHASE");
                order.CompleteOrder();

                // ایجاد ثبت‌نام برای دوره‌های رایگان
                await CreateEnrollmentsForFreeCoursesAsync(request.UserId, cart, cancellationToken);
            }
            else
            {
                // شروع فرآیند پرداخت
                paymentAttempt.StartPayment();
            }

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            // پاک کردن سبد خرید پس از ایجاد سفارش موفق
            cart.Clear();
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            // ایجاد نتیجه
            var result = new CreateCheckoutResult
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                PaymentAttemptId = paymentAttempt.Id,
                TrackingCode = paymentAttempt.TrackingCode ?? string.Empty,
                TotalAmount = paymentAmount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = paymentAttempt.Status,
                RequiresReceiptUpload = paymentAttempt.RequiresReceiptUpload(),
                IsFreePurchase = isFreePurchase,
                Message = isFreePurchase ? "خرید رایگان با موفقیت تکمیل شد" : "سفارش با موفقیت ایجاد شد"
            };

            return OperationResult<CreateCheckoutResult>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<CreateCheckoutResult>.Error($"خطا در ایجاد سفارش: {ex.Message}");
        }
    }

    private async Task CreateEnrollmentsForFreeCoursesAsync(string userId, Domain.Shopping.Cart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            // ایجاد ثبت‌نام برای دوره رایگان
            var enrollment = new Domain.Payments.CourseEnrollment(item.CourseId, userId, 0);
            enrollment.AddPayment(0, "FREE_PURCHASE", Domain.Payments.EnrollmentPaymentMethod.Online);
            
            await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);
        }
        
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);
    }
}