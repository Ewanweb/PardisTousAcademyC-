using MediatR;
using Pardis.Application._Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Users.UpdateUserRole
{
    public class UpdateUserRolesCommand : IRequest<OperationResult<UserResource>>
    {
        public string UserId { get; set; }
        public List<string> Roles { get; set; } // لیست نام نقش‌ها (مثلا ["Admin", "Instructor"])
    }
}
