using Pardis.Domain.Payments;

namespace Pardis.Domain.Dto.Payments;

/// <summary>
/// DTO برای نمایش اطلاعات پرداخت
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public long Amount { get; set; }
    public EnrollmentPaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodDisplay { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string? RecordedByUserId { get; set; }
    public string? RecordedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}