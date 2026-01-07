using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;

namespace Pardis.Application.Users.DeleteAvatar;

public class DeleteAvatarCommand : IRequest<OperationResult<UserProfileDto>>
{
    public string? UserId { get; set; }
}