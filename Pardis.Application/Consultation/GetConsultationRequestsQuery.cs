using MediatR;
using Pardis.Application._Shared;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Consultation;

namespace Pardis.Application.Consultation;

public class GetConsultationRequestsQuery : IRequest<OperationResult<PaginatedList<ConsultationRequest>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public ConsultationStatus? Status { get; set; }
    public string? Search { get; set; }
}
