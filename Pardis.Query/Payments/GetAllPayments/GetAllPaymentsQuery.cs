using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Query.Payments.GetAllPayments;

public class GetAllPaymentsQuery : PagedQuery<ManualPaymentRequestDto>
{
    public string? Search { get; set; }
    public int? Status { get; set; }
}
