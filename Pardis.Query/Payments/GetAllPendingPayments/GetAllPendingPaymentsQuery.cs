using MediatR;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Query.Payments.GetAllPendingPayments;

/// <summary>
/// Query برای دریافت تمام پرداخت‌های در انتظار تایید (PaymentAttempts فقط)
/// </summary>
public class GetAllPendingPaymentsQuery : IRequest<GetAllPendingPaymentsResult>
{
}

/// <summary>
/// نتیجه دریافت تمام پرداخت‌های در انتظار تایید
/// </summary>
public class GetAllPendingPaymentsResult
{
    /// <summary>
    /// پرداخت‌های دستی در انتظار تایید (از PaymentAttempts)
    /// </summary>
    public List<ManualPaymentRequestDto> ManualPayments { get; set; } = new();
    
    /// <summary>
    /// تلاش‌های پرداخت در انتظار تایید
    /// </summary>
    public List<PendingPaymentAttemptDto> PaymentAttempts { get; set; } = new();
}

/// <summary>
/// DTO برای تلاش‌های پرداخت در انتظار تایید
/// </summary>
public class PendingPaymentAttemptDto
{
    public Guid Id { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string ReceiptUrl { get; set; } = string.Empty;
    public DateTime ReceiptUploadedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public List<string> CourseNames { get; set; } = new();
}