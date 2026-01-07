using AutoMapper;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Query.Payments.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, OperationResult<List<ManualPaymentRequestDto>>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IMapper _mapper;

    public GetAllPaymentsQueryHandler(IPaymentAttemptRepository paymentAttemptRepository, IMapper mapper)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _mapper = mapper;
    }
    public async Task<OperationResult<List<ManualPaymentRequestDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentAttemptRepository.GetAll(cancellationToken);
            var dto = _mapper.Map<List<ManualPaymentRequestDto>>(result);
            return OperationResult<List<ManualPaymentRequestDto>>.Success(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult<List<ManualPaymentRequestDto>>.Success(new List<ManualPaymentRequestDto>());
        }
    }
}