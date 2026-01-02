using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Payments.Contracts;
using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;

/// <summary>
/// پردازشگر دستور بررسی پرداخت توسط ادمین
/// </summary>
public class AdminReviewPaymentHandler : IRequestHandler<AdminReviewPaymentCommand, OperationResult<AdminReviewPaymentResult>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public AdminReviewPaymentHandler(
        IPaymentAttemptRepository paymentAttemptRepository,
        IOrderRepository orderRepository,
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _orderRepository = orderRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<AdminReviewPaymentResult>> Handle(AdminReviewPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت تلاش پرداخت
            var paymentAttempt = await _paymentAttemptRepository.GetByIdAsync(request.PaymentAttemptId, cancellationToken);
            if (paymentAttempt == null)
                return OperationResult<AdminReviewPaymentResult>.NotFound("تلاش پرداخت یافت نشد");

            // بررسی وضعیت
            if (!paymentAttempt.RequiresAdminApproval())
                return OperationResult<AdminReviewPaymentResult>.Error("این پرداخت نیاز به تایید ادمین ندارد");

            // بررسی و تایید یا رد پرداخت
            if (request.IsApproved)
            {
                paymentAttempt.ApproveByAdmin(request.AdminUserId);
                
                // تکمیل سفارش
                var order = await _orderRepository.GetByIdAsync(paymentAttempt.OrderId, cancellationToken);
                if (order != null)
                {
                    order.CompleteOrder();
                    await _orderRepository.UpdateAsync(order, cancellationToken);
                    
                    // ایجاد ثبت‌نام برای دوره‌ها
                    await CreateEnrollmentsAsync(order, cancellationToken);
                }
            }
            else
            {
                var reason = request.RejectReason ?? "رسید پرداخت تایید نشد";
                paymentAttempt.RejectByAdmin(request.AdminUserId, reason);
            }

            await _paymentAttemptRepository.UpdateAsync(paymentAttempt, cancellationToken);
            await _paymentAttemptRepository.SaveChangesAsync(cancellationToken);

            // ایجاد نتیجه
            var result = new AdminReviewPaymentResult
            {
                PaymentAttemptId = paymentAttempt.Id,
                TrackingCode = paymentAttempt.TrackingCode ?? string.Empty,
                IsApproved = request.IsApproved,
                AdminDecision = paymentAttempt.AdminDecision ?? string.Empty,
                ReviewedAt = paymentAttempt.AdminReviewedAt ?? DateTime.UtcNow,
                Message = request.IsApproved ? "پرداخت تایید شد" : "پرداخت رد شد"
            };

            return OperationResult<AdminReviewPaymentResult>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<AdminReviewPaymentResult>.Error($"خطا در بررسی پرداخت: {ex.Message}");
        }
    }

    private async Task CreateEnrollmentsAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            // Deserialize cart snapshot to get courses
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(order.CartSnapshot);
            if (cartItems == null) return;

            foreach (var item in cartItems)
            {
                var courseId = Guid.Parse(item.GetProperty("CourseId").GetString() ?? string.Empty);
                var price = item.GetProperty("Price").GetInt64();

                // بررسی اینکه قبلاً ثبت‌نام نشده باشد
                var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(order.UserId, courseId, cancellationToken);
                if (existingEnrollment == null)
                {
                    // ایجاد ثبت‌نام جدید
                    var enrollment = new Domain.Payments.CourseEnrollment(courseId, order.UserId, price);
                    enrollment.AddPayment(price, order.OrderNumber, Domain.Payments.EnrollmentPaymentMethod.Transfer);
                    
                    await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);
                }
            }
            
            await _enrollmentRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the payment approval
            Console.WriteLine($"Error creating enrollments: {ex.Message}");
        }
    }
}