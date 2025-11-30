using MediatR;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetRoles
{
    public class GetRolesQuery : IRequest<List<RoleDto>> { }
}
