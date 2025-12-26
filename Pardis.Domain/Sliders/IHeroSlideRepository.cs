using Pardis.Domain.Sliders;

namespace Pardis.Domain.Sliders
{
    public interface IHeroSlideRepository : IRepository<HeroSlide>
    {
        Task<List<HeroSlide>> GetActiveHeroSlidesAsync(CancellationToken cancellationToken = default);
        Task<List<HeroSlide>> GetHeroSlidesWithFiltersAsync(bool includeInactive = false, bool includeExpired = false, CancellationToken cancellationToken = default);
        Task<HeroSlide?> GetHeroSlideByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}