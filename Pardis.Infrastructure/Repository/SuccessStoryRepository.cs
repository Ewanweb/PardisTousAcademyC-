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
            return await _dbSet
                .Where(x => x.IsActive)
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

            // Note: includeExpired and type parameters are kept for backward compatibility but not used
            // since the simplified slider system doesn't have expiration logic or type classification

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
            // Note: type parameter is kept for backward compatibility but not used
            // since the simplified slider system doesn't have type classification
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Order)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}