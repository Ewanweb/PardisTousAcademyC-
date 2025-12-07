using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsers;

public class CreateUserByAdminCommand : IRequest<OperationResult<UserResource>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Mobile { get; set; }
    public List<string> Roles { get; set; } = new();
}
