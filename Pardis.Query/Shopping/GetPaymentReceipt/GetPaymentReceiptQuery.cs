using MediatR;

namespace Pardis.Query.Shopping.GetPaymentReceipt;

/// <summary>
/// Query برای دریافت رسید پرداخت
/// </summary>
public class GetPaymentReceiptQuery : IRequest<GetPaymentReceiptResult?>
{
    public Guid PaymentAttemptId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه دریافت رسید پرداخت
/// </summary>
public class GetPaymentReceiptResult
{
    public Guid PaymentAttemptId { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public string? ReceiptFileName { get; set; }
    public DateTime? UploadedAt { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
}