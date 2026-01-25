using Pardis.Domain.Dto.Users;

namespace Pardis.Domain.Users;

public interface IAuthLogRepository : IRepository<AuthLog>
{
    Task<List<AuthLog>> GetUserAuthLogs(string userId);
}