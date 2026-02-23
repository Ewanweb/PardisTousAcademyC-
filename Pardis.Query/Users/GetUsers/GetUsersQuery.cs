using MediatR;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Dto.Users;

namespace Pardis.Query.Users.GetUsers;

public class GetUsersQuery : IRequest<PagedResult<UserResource>>
{
    public string? Role { get; set; }
    public bool GetAll { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
