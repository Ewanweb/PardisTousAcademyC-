using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;

namespace Pardis.Query.Payments;

/// <summary>
/// Query دریافت درخواست‌های پرداخت دستی
/// </summary>
public class GetManualPaymentRequestsQuery : IRequest<List<ManualPaymentRequestDto>>
{
    public ManualPaymentStatus? Status { get; set; }
    public string? StudentId { get; set; }
}