using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Audit;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;
using Pardis.Domain.Shopping;
using System.Text.Json;

namespace Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;

public class AdminReviewPaymentHandler : IRequestHandler<AdminReviewPaymentCommand, OperationResult<AdminReviewPaymentResult>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentAuditLogRepository _auditRepository;
    private readonly ILogger<AdminReviewPaymentHandler> _logger;

    public AdminReviewPaymentHandler(
        IPaymentAttemptRepository paymentAttemptRepository,
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        ICourseEnrollmentRepository enrollmentRepository,
        IPaymentAuditLogRepository auditRepository,
        ILogger<AdminReviewPaymentHandler> logger)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _enrollmentRepository = enrollmentRepository;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task<OperationResult<AdminReviewPaymentResult>> Handle(AdminReviewPaymentCommand request, CancellationToken cancellationToken)
    {
        // CRITICAL: Validate idempotency key
        if (string.IsNullOrEmpty(request.IdempotencyKey))
            return OperationResult<AdminReviewPaymentResult>.Error("Idempotency key is required");

        try
        {
            // CRITICAL: Check for duplicate admin action using idempotency key
            var existingAudit = await _auditRepository.Table
                .Where(a => a.IdempotencyKey == request.IdempotencyKey && a.AdminUserId == request.AdminUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingAudit != null)
            {
                _logger.LogWarning("Duplicate admin action detected. IdempotencyKey: {IdempotencyKey}, AdminUserId: {AdminUserId}", 
                    request.IdempotencyKey, request.AdminUserId);
                
                // Return existing result
                var existingPayment = await _paymentAttemptRepository.GetByIdAsync(existingAudit.PaymentAttemptId, cancellationToken);
                if (existingPayment != null)
                {
                    return OperationResult<AdminReviewPaymentResult>.Success(MapToResult(existingPayment, "عملیات قبلاً انجام شده است"));
                }
            }

            // CRITICAL: Use distributed transaction with optimistic locking
            return await _paymentAttemptRepository.ExecuteInTransactionAsync(async (ct) =>
            {
                // Get payment attempt with optimistic locking
                var paymentAttempt = await _paymentAttemptRepository.Table
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.Id == request.PaymentAttemptId, ct);

                if (paymentAttempt == null)
                    return OperationResult<AdminReviewPaymentResult>.NotFound("تلاش پرداخت یافت نشد");

                // Audit log for attempt
                await CreateAuditLog(paymentAttempt, PaymentAuditAction.AdminApproved, 
                    paymentAttempt.Status.ToString(), request, ct);

                if (!paymentAttempt.RequiresAdminApproval())
                    return OperationResult<AdminReviewPaymentResult>.Error("این پرداخت نیاز به تایید ادمین ندارد");

                var enrollmentResults = new List<string>();
                var previousStatus = paymentAttempt.Status.ToString();

                if (request.IsApproved)
                {
                    // CRITICAL: Approve payment with idempotency key
                    paymentAttempt.ApproveByAdmin(request.AdminUserId, request.IdempotencyKey);
                    
                    // Complete order
                    var order = paymentAttempt.Order;
                    if (order != null)
                    {
                        order.CompleteOrder();
                        await _orderRepository.UpdateAsync(order, ct);
                        
                        // CRITICAL: Create enrollments with compensation logic
                        enrollmentResults = await CreateEnrollmentsWithCompensation(order, request, ct);

                        // Clear cart only if enrollments succeed
                        if (enrollmentResults.All(r => !r.Contains("خطا")))
                        {
                            await ClearUserCart(order.UserId, request, ct);
                        }
                    }
                }
                else
                {
                    var reason = request.RejectReason ?? "رسید پرداخت تایید نشد";
                    paymentAttempt.RejectByAdmin(request.AdminUserId, reason, request.IdempotencyKey);
                }

                // CRITICAL: Update with optimistic locking check
                try
                {
                    await _paymentAttemptRepository.UpdateAsync(paymentAttempt, ct);
                    await _paymentAttemptRepository.SaveChangesAsync(ct);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("Optimistic locking conflict for PaymentAttempt {PaymentAttemptId}", request.PaymentAttemptId);
                    return OperationResult<AdminReviewPaymentResult>.Error("عملیات همزمان دیگری در حال انجام است. لطفاً مجدداً تلاش کنید");
                }

                // Final audit log
                await CreateAuditLog(paymentAttempt, 
                    request.IsApproved ? PaymentAuditAction.AdminApproved : PaymentAuditAction.AdminRejected,
                    previousStatus, request, ct);

                var result = MapToResult(paymentAttempt, 
                    request.IsApproved ? "پرداخت تایید شد" : "پرداخت رد شد");
                result.EnrollmentResults = enrollmentResults;

                return OperationResult<AdminReviewPaymentResult>.Success(result);

            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بررسی پرداخت {PaymentAttemptId} توسط ادمین {AdminUserId}", 
                request.PaymentAttemptId, request.AdminUserId);
            return OperationResult<AdminReviewPaymentResult>.Error("خطا در پردازش درخواست");
        }
    }

    private async Task<List<string>> CreateEnrollmentsWithCompensation(Order order, AdminReviewPaymentCommand request, CancellationToken ct)
    {
        var results = new List<string>();
        var createdEnrollments = new List<CourseEnrollment>();

        try
        {
            var cartItems = JsonSerializer.Deserialize<List<CartItemSnapshot>>(order.CartSnapshot) ?? new();
            
            foreach (var item in cartItems)
            {
                try
                {
                    // Check if enrollment already exists (idempotency)
                    var existingEnrollment = await _enrollmentRepository.Table
                        .FirstOrDefaultAsync(e => e.StudentId == order.UserId && e.CourseId == item.CourseId, ct);

                    if (existingEnrollment != null)
                    {
                        results.Add($"دوره {item.CourseTitle}: قبلاً ثبت‌نام شده");
                        continue;
                    }

                    var enrollment = new CourseEnrollment(item.CourseId, order.UserId, item.Price);
                    await _enrollmentRepository.AddAsync(enrollment);
                    createdEnrollments.Add(enrollment);
                    
                    results.Add($"دوره {item.CourseTitle}: ثبت‌نام موفق");

                    // Audit log for enrollment
                    await CreateAuditLog(order.PaymentAttempts.First(), PaymentAuditAction.EnrollmentCreated,
                        "None", request, ct, $"CourseId: {item.CourseId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در ایجاد ثبت‌نام برای دوره {CourseId}", item.CourseId);
                    results.Add($"دوره {item.CourseTitle}: خطا در ثبت‌نام");
                    
                    // CRITICAL: Compensating action - rollback created enrollments
                    foreach (var createdEnrollment in createdEnrollments)
                    {
                        try
                        {
                            _enrollmentRepository.Remove(createdEnrollment);
                        }
                        catch (Exception rollbackEx)
                        {
                            _logger.LogError(rollbackEx, "خطا در rollback ثبت‌نام {EnrollmentId}", createdEnrollment.Id);
                        }
                    }
                    
                    throw; // Re-throw to trigger transaction rollback
                }
            }

            await _enrollmentRepository.SaveChangesAsync(ct);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد ثبت‌نام‌ها برای سفارش {OrderId}", order.Id);
            
            // Audit log for failure
            await CreateAuditLog(order.PaymentAttempts.First(), PaymentAuditAction.EnrollmentFailed,
                "None", request, ct, ex.Message);
            
            throw;
        }
    }

    private async Task ClearUserCart(string userId, AdminReviewPaymentCommand request, CancellationToken ct)
    {
        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId, ct);
            if (cart != null)
            {
                cart.Clear();
                await _cartRepository.UpdateAsync(cart, ct);
                await _cartRepository.SaveChangesAsync(ct);

                // Audit log for cart clearing
                await CreateAuditLog(null, PaymentAuditAction.CartCleared, "Active", request, ct, $"UserId: {userId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پاک کردن سبد خرید کاربر {UserId}", userId);
            // Don't throw - cart clearing failure shouldn't rollback payment approval
        }
    }

    private async Task CreateAuditLog(PaymentAttempt? paymentAttempt, PaymentAuditAction action, 
        string previousStatus, AdminReviewPaymentCommand request, CancellationToken ct, string? additionalData = null)
    {
        try
        {
            var auditLog = new PaymentAuditLog(
                paymentAttempt?.Id ?? Guid.Empty,
                paymentAttempt?.UserId ?? string.Empty,
                action,
                previousStatus,
                paymentAttempt?.Status.ToString() ?? "Unknown",
                paymentAttempt?.Amount ?? 0,
                request.IpAddress,
                request.UserAgent,
                request.AdminUserId,
                request.RejectReason,
                request.IdempotencyKey,
                additionalData);

            await _auditRepository.AddAsync(auditLog);
            await _auditRepository.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد audit log برای عملیات {Action}", action);
            // Don't throw - audit failure shouldn't stop the main operation
        }
    }

    private AdminReviewPaymentResult MapToResult(PaymentAttempt paymentAttempt, string message)
    {
        return new AdminReviewPaymentResult
        {
            PaymentAttemptId = paymentAttempt.Id,
            TrackingCode = paymentAttempt.TrackingCode ?? string.Empty,
            IsApproved = paymentAttempt.Status == PaymentAttemptStatus.Paid,
            AdminDecision = paymentAttempt.AdminDecision ?? string.Empty,
            ReviewedAt = paymentAttempt.AdminReviewedAt ?? DateTime.UtcNow,
            Message = message
        };
    }
}

// Helper class for cart snapshot deserialization
public class CartItemSnapshot
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public long Price { get; set; }
}
