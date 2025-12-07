using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetRoles
{
    public class GetRolesHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
    {
        private readonly RoleManager<Role> _roleManager;

        public GetRolesHandler(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken token)
        {
            return await _roleManager.Roles
                .Select(r => new RoleDto
                {
                    Name = r.Name,
                    Description = r.Description ?? r.Name // اگر توضیحات نداشت، اسمش را بگذار
                })
                .ToListAsync(token);
        }
    }
}
