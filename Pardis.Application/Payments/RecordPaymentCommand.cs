using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Payments;

namespace Pardis.Application.Payments;

/// <summary>
/// Command برای ثبت پرداخت دستی توسط ادمین
/// </summary>
public class RecordPaymentCommand : IRequest<OperationResult<bool>>
{
    public Guid EnrollmentId { get; set; }
    public long Amount { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public EnrollmentPaymentMethod Method { get; set; } = EnrollmentPaymentMethod.Cash;
    public string? Notes { get; set; }
}
