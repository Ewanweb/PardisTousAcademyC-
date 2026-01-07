using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Query.Users.GetProfile;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, OperationResult<UserProfileDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetUserProfileHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<OperationResult<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserId))
            return OperationResult<UserProfileDto>.Error("شناسه کاربر یافت نشد");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return OperationResult<UserProfileDto>.NotFound("کاربر یافت نشد");

        var profileDto = _mapper.Map<UserProfileDto>(user);

        if (!string.IsNullOrEmpty(profileDto.AvatarUrl))
        {
            profileDto.AvatarUrl = $"{profileDto.AvatarUrl}?v={user.AvatarUpdatedAt?.Ticks ?? DateTime.UtcNow.Ticks}";
        }

        return OperationResult<UserProfileDto>.Success(profileDto);
    }
}
