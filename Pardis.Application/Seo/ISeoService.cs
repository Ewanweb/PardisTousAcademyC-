using Pardis.Domain.Seo;
using Pardis.Domain.Dto.Seo;

namespace Pardis.Application.Seo
{
    public interface ISeoService
    {
        Task<SeoDto> ResolveSeoAsync(ISeoEntity entity, SeoContext context);
        Task<SeoDto> ResolveSeoBySlugAsync(SeoEntityType entityType, string slug, SeoContext context);
        
        string GenerateSlug(string title, string? existingSlug = null);
        Task<string> GenerateUniqueSlugAsync(string title, SeoEntityType entityType, string? existingSlug = null);
        
        string BuildCanonicalUrl(string path, Dictionary<string, string>? queryParams = null, string? baseUrl = null);
        string NormalizeUrl(string url);
        
        bool ShouldIndex(string path, Dictionary<string, string>? queryParams = null);
        string GetRobotsDirective(string path, Dictionary<string, string>? queryParams = null);
        
        Task InvalidateCacheAsync(SeoEntityType entityType, string slug);
    }

}
