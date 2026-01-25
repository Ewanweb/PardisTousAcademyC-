using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Payments.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, OperationResult<List<ManualPaymentRequestDto>>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetAllPaymentsQueryHandler(IPaymentAttemptRepository paymentAttemptRepository, IMapper mapper, UserManager<User> userManager)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _paymentAttemptRepository = paymentAttemptRepository;
        _mapper = mapper;
        _userManager = userManager;
    }
    public async Task<OperationResult<List<ManualPaymentRequestDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentAttemptRepository.GetAll(cancellationToken);
            var dto = _mapper.Map<List<ManualPaymentRequestDto>>(result);
            foreach (var item in dto)
            {
                User? admin = null;
                if (!string.IsNullOrEmpty(item.AdminReviewedBy))
                     admin = await _userManager.FindByIdAsync(item.AdminReviewedBy);

                item.AdminReviewerName = admin.FullName;
            }
            return OperationResult<List<ManualPaymentRequestDto>>.Success(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult<List<ManualPaymentRequestDto>>.Success(new List<ManualPaymentRequestDto>());
        }
    }
}