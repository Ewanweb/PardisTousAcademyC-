using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;

namespace Pardis.Query.Payments;

/// <summary>
/// Handler برای دریافت درخواست‌های پرداخت دستی
/// </summary>
public class GetManualPaymentRequestsHandler : IRequestHandler<GetManualPaymentRequestsQuery, List<ManualPaymentRequestDto>>
{
    private readonly IManualPaymentRequestRepository _manualPaymentRepository;
    private readonly IMapper _mapper;

    public GetManualPaymentRequestsHandler(IManualPaymentRequestRepository manualPaymentRepository, IMapper mapper)
    {
        _manualPaymentRepository = manualPaymentRepository;
        _mapper = mapper;
    }

    public async Task<List<ManualPaymentRequestDto>> Handle(GetManualPaymentRequestsQuery request, CancellationToken cancellationToken)
    {
        List<ManualPaymentRequest> requests;

        if (request.Status.HasValue)
        {
            requests = await _manualPaymentRepository.GetRequestsByStatusAsync(request.Status.Value, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.StudentId))
        {
            requests = await _manualPaymentRepository.GetRequestsByStudentAsync(request.StudentId, cancellationToken);
        }
        else
        {
            requests = await _manualPaymentRepository.GetAllWithDetailsAsync(cancellationToken);
        }

        // استفاده از AutoMapper برای تبدیل به DTO
        return _mapper.Map<List<ManualPaymentRequestDto>>(requests);
    }
}