using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;

namespace Pardis.Application.Users.Auth;

public class LoginUserCommand : IRequest<OperationResult<AuthResultDto>>
{
    public required string Mobile { get; set; }
    public required string Password { get; set; }
}