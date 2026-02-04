using Pardis.Domain.Shopping;

namespace Pardis.Query.Shopping.GetPaymentAttempt;

public class GetPaymentAttemptResult
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public long Amount { get; set; }
    public PaymentAttemptStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public PaymentMethod Method { get; set; }
    public string MethodText { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
    public string? RejectReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool RequiresReceiptUpload { get; set; }
}
