using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;

namespace Pardis.Application.Users.Auth
{
    public class RegisterUserCommand : IRequest<OperationResult<AuthResultDto>>
    {
        public string? Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Mobile { get; set; }
    }
}
