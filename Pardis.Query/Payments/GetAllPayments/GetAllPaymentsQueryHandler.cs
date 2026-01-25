using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.Pagination;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Payments.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, OperationResult<PagedResult<ManualPaymentRequestDto>>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetAllPaymentsQueryHandler(IPaymentAttemptRepository paymentAttemptRepository, IMapper mapper, UserManager<User> userManager)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<OperationResult<PagedResult<ManualPaymentRequestDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _paymentAttemptRepository.GetAllPagedAsync(
                request.Pagination,
                request.Search,
                request.Status,
                cancellationToken);

            var dto = _mapper.Map<List<ManualPaymentRequestDto>>(pagedResult.Items);
            foreach (var item in dto)
            {
                User? admin = null;
                if (!string.IsNullOrEmpty(item.AdminReviewedBy))
                    admin = await _userManager.FindByIdAsync(item.AdminReviewedBy);

                item.AdminReviewerName = admin?.FullName;
            }

            var response = new PagedResult<ManualPaymentRequestDto>
            {
                Items = dto,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                HasNext = pagedResult.HasNext,
                HasPrev = pagedResult.HasPrev,
                Stats = pagedResult.Stats
            };

            return OperationResult<PagedResult<ManualPaymentRequestDto>>.Success(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult<PagedResult<ManualPaymentRequestDto>>.Success(new PagedResult<ManualPaymentRequestDto>());
        }
    }
}
