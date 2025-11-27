using Pardis.Domain.Users;

namespace Pardis.Infrastructure.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}