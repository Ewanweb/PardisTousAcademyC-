using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Application.Payments.ManualPayment;

/// <summary>
/// Command ایجاد درخواست پرداخت دستی
/// </summary>
public class CreateManualPaymentCommand : IRequest<OperationResult<ManualPaymentRequestDto>>
{
    public Guid CourseId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public long Amount { get; set; }
}