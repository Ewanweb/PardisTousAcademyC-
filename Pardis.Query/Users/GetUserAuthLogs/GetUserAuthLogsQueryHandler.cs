using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Query.Users.GetUserAuthLogs;

public class GetUserAuthLogsQueryHandler : IRequestHandler<GetUserAuthLogsQuery, List<AuthLogDTO>>
{
    private readonly IAuthLogRepository _authLogRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public GetUserAuthLogsQueryHandler(IAuthLogRepository authLogRepository, UserManager<User> userManager, IMapper mapper)
    {
        _authLogRepository = authLogRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<AuthLogDTO>> Handle(GetUserAuthLogsQuery request, CancellationToken cancellationToken)
    {
        var data = await _authLogRepository.GetUserAuthLogs(request.UserId);

        var result = _mapper.Map<List<AuthLogDTO>>(data);

        return result;
    }
}