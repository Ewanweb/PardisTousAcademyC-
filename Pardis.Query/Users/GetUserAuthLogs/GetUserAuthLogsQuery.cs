using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Pardis.Domain.Dto.Users;

namespace Pardis.Query.Users.GetUserAuthLogs
{
    public record GetUserAuthLogsQuery(string UserId) : IRequest<List<AuthLogDTO>>;
}
