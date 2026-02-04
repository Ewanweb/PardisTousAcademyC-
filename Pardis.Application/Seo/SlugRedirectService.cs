using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Seo;

namespace Pardis.Application.Seo
{
    public interface ISlugRedirectService
    {
        Task<string?> GetRedirectUrlAsync(string oldSlug, SeoEntityType entityType);
        Task CreateRedirectAsync(string oldSlug, string newSlug, SeoEntityType entityType);
        Task<bool> SlugExistsAsync(string slug, SeoEntityType entityType);
    }

    public class SlugRedirectService : ISlugRedirectService
    {
        private readonly DbContext _context;

        public SlugRedirectService(DbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetRedirectUrlAsync(string oldSlug, SeoEntityType entityType)
        {
            var redirect = await _context.Set<SlugRedirect>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.OldSlug == oldSlug && 
                                         r.EntityType == entityType && 
                                         r.IsActive);

            return redirect?.NewSlug;
        }

        public async Task CreateRedirectAsync(string oldSlug, string newSlug, SeoEntityType entityType)
        {
            if (oldSlug == newSlug) return;

            var existingRedirect = await _context.Set<SlugRedirect>()
                .FirstOrDefaultAsync(r => r.OldSlug == oldSlug && r.EntityType == entityType);

            if (existingRedirect != null)
            {
                existingRedirect.NewSlug = newSlug;
                existingRedirect.CreatedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                var redirect = new SlugRedirect(oldSlug, newSlug, entityType);
                _context.Set<SlugRedirect>().Add(redirect);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug, SeoEntityType entityType)
        {
            if (entityType == SeoEntityType.Category)
                return await _context.Set<Domain.Categories.Category>()
                    .AnyAsync(e => EF.Property<string>(e, "Slug") == slug);

            if (entityType == SeoEntityType.Course)
                return await _context.Set<Domain.Courses.Course>()
                    .AnyAsync(e => EF.Property<string>(e, "Slug") == slug);

            if (entityType == SeoEntityType.Page)
                return await _context.Set<Domain.Pages.Page>()
                    .AnyAsync(e => EF.Property<string>(e, "Slug") == slug);

            return false;
        }
    }
}
