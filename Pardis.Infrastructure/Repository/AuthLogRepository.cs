using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Infrastructure.Repository
{
    public class AuthLogRepository : Repository<AuthLog>, IAuthLogRepository
    {

        public AuthLogRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<List<AuthLog>> GetUserAuthLogs(string userId)
        {
           return  await _context.AuthLogs.Where(x => x.UserId == userId).Take(5).ToListAsync();
        }
    }
}
