using MediatR;
using Pardis.Application._Shared;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Consultation;

namespace Pardis.Application.Consultation;

public class GetConsultationRequestsHandler : IRequestHandler<GetConsultationRequestsQuery, OperationResult<PaginatedList<ConsultationRequest>>>
{
    private readonly IConsultationRequestRepository _repository;

    public GetConsultationRequestsHandler(IConsultationRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult<PaginatedList<ConsultationRequest>>> Handle(GetConsultationRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var allRequests = await _repository.GetAllAsync();

            // Filter by status
            if (request.Status.HasValue)
            {
                allRequests = allRequests.Where(r => r.Status == request.Status.Value).ToList();
            }

            // Filter by search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                allRequests = allRequests.Where(r =>
                    r.FullName.ToLower().Contains(searchLower) ||
                    r.PhoneNumber.Contains(searchLower) ||
                    (r.Email != null && r.Email.ToLower().Contains(searchLower)) ||
                    (r.CourseName != null && r.CourseName.ToLower().Contains(searchLower))
                ).ToList();
            }

            // Pagination
            var totalCount = allRequests.Count;
            var items = allRequests
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var paginatedList = new PaginatedList<ConsultationRequest>(
                items,
                request.Page,
                request.PageSize,
                totalCount
            );

            return OperationResult<PaginatedList<ConsultationRequest>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return OperationResult<PaginatedList<ConsultationRequest>>.Error($"خطا در دریافت درخواست‌های مشاوره: {ex.Message}");
        }
    }
}
