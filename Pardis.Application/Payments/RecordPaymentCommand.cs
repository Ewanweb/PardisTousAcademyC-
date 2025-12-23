using MediatR;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Application.Payments;

/// <summary>
/// Command برای ثبت پرداخت جدید
/// </summary>
public class RecordPaymentCommand : IRequest<PaymentDto>
{
    public Guid EnrollmentId { get; set; }
    public long Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PaymentDate { get; set; }
    public string RecordedByUserId { get; set; } = string.Empty;
}