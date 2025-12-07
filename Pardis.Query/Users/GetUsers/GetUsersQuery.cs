using MediatR;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsers;

public class GetUsersQuery : IRequest<List<UserResource>>
{
    public string? Role { get; set; }
    public bool GetAll { get; set; }
}
