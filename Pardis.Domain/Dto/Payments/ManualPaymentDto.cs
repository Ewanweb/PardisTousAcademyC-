using Pardis.Domain.Payments;

namespace Pardis.Domain.Dto.Payments;

/// <summary>
/// DTO درخواست پرداخت دستی
/// </summary>
public class ManualPaymentRequestDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public long Amount { get; set; }
    public ManualPaymentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string? ReceiptFileUrl { get; set; }
    public string? ReceiptFileName { get; set; }
    public DateTime? ReceiptUploadedAt { get; set; }
    public string? AdminReviewedBy { get; set; }
    public string? AdminReviewerName { get; set; }
    public DateTime? AdminReviewedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO ایجاد درخواست پرداخت دستی
/// </summary>
public class CreateManualPaymentRequestDto
{
    public Guid CourseId { get; set; }
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