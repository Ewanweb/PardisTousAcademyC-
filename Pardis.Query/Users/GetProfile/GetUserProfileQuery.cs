using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;

namespace Pardis.Query.Users.GetProfile;

public class GetUserProfileQuery : IRequest<OperationResult<UserProfileDto>>
{
    public string? UserId { get; set; }
}