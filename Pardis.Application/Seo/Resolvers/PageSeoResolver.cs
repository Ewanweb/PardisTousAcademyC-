using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Pages;
using Pardis.Domain.Seo;

namespace Pardis.Application.Seo.Resolvers
{
    public class PageSeoResolver : ISeoResolver<Page>, ISeoResolverBase
    {
        private readonly DbContext _context;
        private readonly ISeoService _seoService;

        public SeoEntityType EntityType => SeoEntityType.Page;

        public PageSeoResolver(DbContext context, ISeoService seoService)
        {
            _context = context;
            _seoService = seoService;
        }

        public async Task<SeoDto> ResolveAsync(Page page, SeoContext context)
        {
            var seo = page.Seo;

            var title = !string.IsNullOrEmpty(seo.MetaTitle) ? seo.MetaTitle : GenerateDefaultTitle(page, context);
            var description = !string.IsNullOrEmpty(seo.MetaDescription) ? seo.MetaDescription : GenerateDefaultDescription(page, context);
            var canonicalUrl = !string.IsNullOrEmpty(seo.CanonicalUrl)
                ? seo.CanonicalUrl
                : _seoService.BuildCanonicalUrl($"/{page.Slug}", context.QueryParams, context.BaseUrl);

            return new SeoDto
            {
                MetaTitle = title,
                MetaDescription = description,
                Keywords = seo.Keywords,
                CanonicalUrl = canonicalUrl,
                RobotsContent = GetRobotsDirective(page, context),
                LastModified = page.UpdatedAt,

                OgTitle = seo.OpenGraphTitle ?? title,
                OgDescription = seo.OpenGraphDescription ?? description,
                OgImage = seo.OpenGraphImage ?? GetDefaultImage(context),
                OgType = seo.OpenGraphType,
                OgSiteName = "?????? ????? ???",
                OgLocale = page.Language == "en" ? "en_US" : "fa_IR",

                TwitterTitle = seo.TwitterTitle ?? title,
                TwitterDescription = seo.TwitterDescription ?? description,
                TwitterImage = seo.TwitterImage ?? seo.OpenGraphImage ?? GetDefaultImage(context),
                TwitterCard = seo.TwitterCardType,

                JsonLdSchemas = GenerateJsonLdSchemas(page, context),
                Breadcrumbs = GenerateBreadcrumbs(page, context),
                Direction = page.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = canonicalUrl
            };
        }

        public async Task<SeoDto> ResolveBySlugAsync(string slug, SeoContext context)
        {
            var page = await _context.Set<Page>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

            if (page == null)
            {
                throw new InvalidOperationException($"Page not found: {slug}");
            }

            return await ResolveAsync(page, context);
        }

        public SeoDto GenerateDefaultSeo(Page page, SeoContext context)
        {
            var canonicalUrl = BuildCanonicalUrl(page, context);
            var title = GenerateDefaultTitle(page, context);
            var description = GenerateDefaultDescription(page, context);

            return new SeoDto
            {
                MetaTitle = title,
                MetaDescription = description,
                Keywords = page.Seo.Keywords,
                CanonicalUrl = canonicalUrl,
                RobotsContent = GetRobotsDirective(page, context),
                OgTitle = page.Seo.OpenGraphTitle ?? title,
                OgDescription = page.Seo.OpenGraphDescription ?? description,
                OgImage = page.Seo.OpenGraphImage ?? GetDefaultImage(context),
                OgType = page.Seo.OpenGraphType,
                OgSiteName = "?????? ????? ???",
                OgLocale = page.Language == "en" ? "en_US" : "fa_IR",
                TwitterTitle = page.Seo.TwitterTitle ?? title,
                TwitterDescription = page.Seo.TwitterDescription ?? description,
                TwitterImage = page.Seo.TwitterImage ?? page.Seo.OpenGraphImage ?? GetDefaultImage(context),
                TwitterCard = page.Seo.TwitterCardType,
                JsonLdSchemas = GenerateJsonLdSchemas(page, context),
                Breadcrumbs = GenerateBreadcrumbs(page, context),
                Direction = page.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = canonicalUrl
            };
        }

        public string BuildCanonicalUrl(Page page, SeoContext context)
        {
            return _seoService.BuildCanonicalUrl($"/{page.Slug}", context.QueryParams, context.BaseUrl);
        }

        public List<object> GenerateJsonLdSchemas(Page page, SeoContext context)
        {
            var schemas = new List<object>();

            if (!string.IsNullOrEmpty(page.Seo.JsonLdSchemas))
            {
                var existingSchemas = page.Seo.GetJsonLdSchemas();
                schemas.AddRange(existingSchemas);
            }

            var webPageSchema = new
            {
                context = "https://schema.org",
                type = "WebPage",
                name = page.Title,
                description = page.Description ?? page.Title,
                url = _seoService.BuildCanonicalUrl($"/{page.Slug}", null, context.BaseUrl),
                inLanguage = page.Language,
                isPartOf = new
                {
                    type = "WebSite",
                    name = "?????? ????? ???",
                    url = context.BaseUrl
                },
                datePublished = page.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dateModified = page.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            schemas.Add(webPageSchema);

            if (page.PageType == "Contact")
            {
                var organizationSchema = new
                {
                    context = "https://schema.org",
                    type = "Organization",
                    name = "?????? ????? ???",
                    url = context.BaseUrl,
                    contactPoint = new
                    {
                        type = "ContactPoint",
                        contactType = "customer service",
                        availableLanguage = new[] { "Persian", "English" }
                    }
                };

                schemas.Add(organizationSchema);
            }

            return schemas;
        }

        public List<BreadcrumbItem> GenerateBreadcrumbs(Page page, SeoContext context)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new() { Name = "????", Url = context.BaseUrl, Position = 1 },
                new() { Name = page.Title, Url = $"{context.BaseUrl}/{page.Slug}", Position = 2 }
            };

            return breadcrumbs;
        }

        private string GenerateDefaultTitle(Page page, SeoContext context)
        {
            var baseTitle = page.Title;

            return page.PageType switch
            {
                "About" => $"{baseTitle} | ?????? ??",
                "Contact" => $"{baseTitle} | ???? ?? ??",
                "Landing" => baseTitle,
                _ => $"{baseTitle} | ?????? ????? ???"
            };
        }

        private string GenerateDefaultDescription(Page page, SeoContext context)
        {
            if (!string.IsNullOrEmpty(page.Description))
                return page.Description;

            return page.PageType switch
            {
                "About" => "?????? ????? ??? - ????? ????? ???????? ?????? ????? ? ????? ????",
                "Contact" => "?? ?????? ????? ??? ?? ???? ?????. ??????? ???????? ???? ? ??????? ????",
                _ => $"{page.Title} - ?????? ????? ???"
            };
        }

        private string GetRobotsDirective(Page page, SeoContext context)
        {
            if (page.Seo.NoIndex || !page.IsPublished)
                return "noindex, nofollow";

            return _seoService.GetRobotsDirective($"/{page.Slug}", context.QueryParams);
        }

        private string GetDefaultImage(SeoContext context)
        {
            return $"{context.BaseUrl}/images/default-og-image.jpg";
        }
    }
}

