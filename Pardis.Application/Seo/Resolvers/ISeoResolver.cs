using Pardis.Domain.Seo;
using Pardis.Domain.Dto.Seo;

namespace Pardis.Application.Seo.Resolvers
{
    public interface ISeoResolver<T> where T : ISeoEntity
    {
        Task<SeoDto> ResolveAsync(T entity, SeoContext context);
        SeoDto GenerateDefaultSeo(T entity, SeoContext context);
        string BuildCanonicalUrl(T entity, SeoContext context);
        List<object> GenerateJsonLdSchemas(T entity, SeoContext context);
        List<BreadcrumbItem> GenerateBreadcrumbs(T entity, SeoContext context);
    }

    public interface ISeoResolverBase
    {
        Task<SeoDto> ResolveBySlugAsync(string slug, SeoContext context);
        SeoEntityType EntityType { get; }
    }
}
