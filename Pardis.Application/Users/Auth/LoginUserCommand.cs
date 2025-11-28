using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Users.Auth;

public class LoginUserCommand : IRequest<OperationResult>
{
    public string Email { get; set; }
    public string Password { get; set; }
}