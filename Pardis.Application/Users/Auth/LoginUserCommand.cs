using MediatR;
using Pardis.Application._Shared;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Users.Auth;

public class LoginUserCommand : IRequest<OperationResult<AuthResultDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}