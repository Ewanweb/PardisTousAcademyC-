using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Seo;
using Pardis.Domain.Seo;
using System.Text.Json;
using SeoDto = Pardis.Domain.Dto.Seo.SeoDto;

namespace Endpoints.Api.ViewComponents
{
    public class SeoHeadViewComponent : ViewComponent
    {
        private readonly Pardis.Application.Seo.ISeoService _seoService;

        public SeoHeadViewComponent(Pardis.Application.Seo.ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ISeoEntity? entity = default,
            SeoEntityType? entityType = default,
            string? slug = null,
            string language = "fa")
        {
            var context = new SeoContext
            {
                BaseUrl = $"{Request.Scheme}://{Request.Host}",
                Language = language,
                CurrentPath = Request.Path,
                QueryParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
                Page = GetPageFromQuery(),
                IsPreview = Request.Query.ContainsKey("preview"),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            SeoDto seoDto;

            if (entity != null)
            {
                seoDto = await _seoService.ResolveSeoAsync(entity, context);
            }
            else if (entityType.HasValue && !string.IsNullOrEmpty(slug))
            {
                try
                {
                    seoDto = await _seoService.ResolveSeoBySlugAsync(entityType.Value, slug, context);
                }
                catch (InvalidOperationException)
                {
                    seoDto = GetFallbackSeo(context);
                }
            }
            else
            {
                seoDto = GetFallbackSeo(context);
            }

            return View(seoDto);
        }

        private int? GetPageFromQuery()
        {
            if (Request.Query.TryGetValue("page", out var pageValue) && 
                int.TryParse(pageValue, out var page) && page > 0)
            {
                return page;
            }
            return null;
        }

        private SeoDto GetFallbackSeo(SeoContext context)
        {
            return new SeoDto
            {
                MetaTitle = "آکادمی پردیس توس",
                MetaDescription = "دوره‌های پروژه‌محور برنامه‌نویسی، طراحی وب و مهارت‌های دیجیتال با پشتیبانی منتور و مدرک معتبر",
                CanonicalUrl = $"{context.BaseUrl}{context.CurrentPath}",
                RobotsContent = "index, follow",
                OgTitle = "آکادمی پردیس توس",
                OgDescription = "دوره‌های پروژه‌محور برنامه‌نویسی، طراحی وب و مهارت‌های دیجیتال",
                OgType = "website",
                OgSiteName = "آکادمی پردیس توس",
                OgLocale = context.Language == "en" ? "en_US" : "fa_IR",
                TwitterTitle = "آکادمی پردیس توس",
                TwitterDescription = "دوره‌های پروژه‌محور برنامه‌نویسی، طراحی وب و مهارت‌های دیجیتال",
                TwitterCard = "summary_large_image",
                Direction = context.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = $"{context.BaseUrl}{context.CurrentPath}"
            };
        }
    }
}
