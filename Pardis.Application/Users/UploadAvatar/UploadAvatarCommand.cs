using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;

namespace Pardis.Application.Users.UploadAvatar;

public class UploadAvatarCommand : IRequest<OperationResult<UserProfileDto>>
{
    public required IFormFile Avatar { get; set; }
    public string? UserId { get; set; }
}