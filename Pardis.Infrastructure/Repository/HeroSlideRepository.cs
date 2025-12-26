using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Sliders;

namespace Pardis.Infrastructure.Repository
{
    public class HeroSlideRepository : Repository<HeroSlide>, IHeroSlideRepository
    {
        public HeroSlideRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<HeroSlide>> GetActiveHeroSlidesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            
            return await _dbSet
                .Where(x => x.IsActive && (x.IsPermanent || !x.ExpiresAt.HasValue || x.ExpiresAt.Value > now))
                .OrderBy(x => x.Order)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<HeroSlide>> GetHeroSlidesWithFiltersAsync(bool includeInactive = false, bool includeExpired = false, CancellationToken cancellationToken = default)
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

            // مرتب‌سازی
            query = query.OrderBy(x => x.Order).ThenByDescending(x => x.CreatedAt);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<HeroSlide?> GetHeroSlideByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}