using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Application.Payments.ManualPayment.ReviewManualPayment;

/// <summary>
/// Command تایید/رد پرداخت دستی توسط ادمین
/// </summary>
public class ReviewManualPaymentCommand : IRequest<OperationResult<ManualPaymentRequestDto>>
{
    public Guid PaymentRequestId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? RejectReason { get; set; }
}