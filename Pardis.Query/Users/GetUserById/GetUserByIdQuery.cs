using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUserById;

public class GetUserByIdQuery : IRequest<UserResource>
{
    public string Id { get; set; }
}