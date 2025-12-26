using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Sliders;

namespace Pardis.Infrastructure.Repository
{
    public class SuccessStoryRepository : Repository<SuccessStory>, ISuccessStoryRepository
    {
        public SuccessStoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<SuccessStory>> GetActiveSuccessStoriesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            
            return await _dbSet
                .Where(x => x.IsActive && (x.IsPermanent || !x.ExpiresAt.HasValue || x.ExpiresAt.Value > now))
                .OrderBy(x => x.Order)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SuccessStory>> GetSuccessStoriesWithFiltersAsync(bool includeInactive = false, bool includeExpired = false, string? type = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            // فیلتر بر اساس وضعیت فعال/غیرفعال
            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            // فیلتر بر اساس انقضا
            if (!includeExpired)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.IsPermanent || !x.ExpiresAt.HasValue || x.ExpiresAt.Value > now);
            }

            // فیلتر بر اساس نوع (اختیاری)
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }

            // مرتب‌سازی
            query = query.OrderBy(x => x.Order).ThenByDescending(x => x.CreatedAt);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<SuccessStory?> GetSuccessStoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<SuccessStory>> GetSuccessStoriesByTypeAsync(string type, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            
            return await _dbSet
                .Where(x => x.IsActive && 
                           x.Type == type && 
                           (x.IsPermanent || !x.ExpiresAt.HasValue || x.ExpiresAt.Value > now))
                .OrderBy(x => x.Order)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}