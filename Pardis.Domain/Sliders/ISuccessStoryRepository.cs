using Pardis.Domain.Sliders;

namespace Pardis.Domain.Sliders
{
    public interface ISuccessStoryRepository : IRepository<SuccessStory>
    {
        Task<List<SuccessStory>> GetActiveSuccessStoriesAsync(CancellationToken cancellationToken = default);
        Task<List<SuccessStory>> GetSuccessStoriesWithFiltersAsync(bool includeInactive = false, bool includeExpired = false, string? type = null, CancellationToken cancellationToken = default);
        Task<SuccessStory?> GetSuccessStoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<SuccessStory>> GetSuccessStoriesByTypeAsync(string type, CancellationToken cancellationToken = default);
    }
}