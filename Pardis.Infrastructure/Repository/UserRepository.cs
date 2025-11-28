using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Users;

namespace Pardis.Infrastructure.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> EmailIsExist(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> MobileIsExist(string phone)
    {
        return await _context.Users.AnyAsync(x => x.Mobile == phone);
    }

    public async Task<bool> UserIsExist(string userName)
    {
        return await _context.Users.AnyAsync(x => x.UserName == userName);
    }
}