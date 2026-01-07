using Pardis.Domain.Shopping;

namespace Pardis.Domain.Dto.Payments;

/// <summary>
/// DTO درخواست پرداخت دستی - ساده شده برای PaymentAttempt
/// </summary>
public class ManualPaymentRequestDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public long Amount { get; set; }
    public PaymentAttemptStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string? ReceiptImageUrl { get; set; }
    public string? ReceiptFileName { get; set; }
    public DateTime? ReceiptUploadedAt { get; set; }
    public string? AdminReviewedBy { get; set; }
    public string? AdminReviewerName { get; set; }
    public DateTime? AdminReviewedAt { get; set; }
    public string? AdminDecision { get; set; }
    public string? FailureReason { get; set; }
    public string? TrackingCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO ایجاد درخواست پرداخت دستی - ساده شده
/// </summary>
public class CreateManualPaymentRequestDto
{
    public Guid OrderId { get; set; }
    public long Amount { get; set; }
}

/// <summary>
/// DTO تایید/رد پرداخت دستی
/// </summary>
public class ReviewManualPaymentDto
{
    public bool IsApproved { get; set; }
    public string? RejectReason { get; set; }
}

/// <summary>
/// DTO اطلاعات کارت مقصد
/// </summary>
public class PaymentCardInfoDto
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? Description { get; set; }
}