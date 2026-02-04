using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Seo;
using Pardis.Domain.Seo;
using Pardis.Domain.Dto.Seo;

namespace Api.Controllers
{
    /// <summary>
    /// SEO controller for handling SEO metadata and sitemap generation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SeoController : ControllerBase
    {
        private readonly Pardis.Application.Seo.ISeoService _seoService;

        /// <summary>
        /// Initializes a new instance of the SeoController
        /// </summary>
        /// <param name="seoService">SEO service</param>
        public SeoController(Pardis.Application.Seo.ISeoService seoService)
        {
            _seoService = seoService;
        }

        /// <summary>
        /// Get SEO metadata for a specific entity
        /// </summary>
        /// <param name="entityType">Type of entity (category, course, page)</param>
        /// <param name="slug">Entity slug</param>
        /// <param name="language">Language code (default: fa)</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="isPreview">Whether this is a preview request</param>
        /// <returns>SEO metadata</returns>
        [HttpGet("{entityType}/{slug}")]
        public async Task<ActionResult<SeoDto>> GetSeo(
            string entityType, 
            string slug,
            [FromQuery] string language = "fa",
            [FromQuery] int? page = null,
            [FromQuery] bool isPreview = false)
        {
            if (!Enum.TryParse<SeoEntityType>(entityType, true, out var parsedEntityType))
            {
                return BadRequest($"Invalid entity type: {entityType}");
            }

            var context = new SeoContext
            {
                BaseUrl = $"{Request.Scheme}://{Request.Host}",
                Language = language,
                CurrentPath = Request.Path + Request.QueryString,
                QueryParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
                Page = page,
                IsPreview = isPreview,
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            try
            {
                var seoDto = await _seoService.ResolveSeoBySlugAsync(parsedEntityType, slug, context);
                return Ok(seoDto);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"Entity not found: {entityType}/{slug}");
            }
        }

        /// <summary>
        /// Generate XML sitemap
        /// </summary>
        /// <returns>XML sitemap</returns>
        [HttpGet("sitemap.xml")]
        [Produces("application/xml")]
        public async Task<IActionResult> GetSitemap()
        {
            var sitemap = await GenerateSitemapAsync();
            return Content(sitemap, "application/xml");
        }

        /// <summary>
        /// Generate robots.txt file
        /// </summary>
        /// <returns>Robots.txt content</returns>
        [HttpGet("robots.txt")]
        [Produces("text/plain")]
        public IActionResult GetRobots()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var robots = $@"User-agent: *
Allow: /

Sitemap: {baseUrl}/api/seo/sitemap.xml

Disallow: /admin/
Disallow: /profile/
Disallow: /cart
Disallow: /orders
Disallow: /checkout
Disallow: /login
Disallow: /register

Allow: /
Allow: /course/
Allow: /category/
Allow: /about
Allow: /contact

Crawl-delay: 1";

            return Content(robots, "text/plain");
        }

        private async Task<string> GenerateSitemapAsync()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var sitemap = new System.Text.StringBuilder();
            
            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Homepage
            sitemap.AppendLine($@"  <url>
    <loc>{baseUrl}/</loc>
    <lastmod>{DateTimeOffset.UtcNow:yyyy-MM-dd}</lastmod>
    <changefreq>daily</changefreq>
    <priority>1.0</priority>
  </url>");

            // Static pages
            var staticPages = new[]
            {
                new { Url = "/about", Priority = "0.8", ChangeFreq = "monthly" },
                new { Url = "/contact", Priority = "0.8", ChangeFreq = "monthly" }
            };

            foreach (var page in staticPages)
            {
                sitemap.AppendLine($@"  <url>
    <loc>{baseUrl}{page.Url}</loc>
    <lastmod>{DateTimeOffset.UtcNow:yyyy-MM-dd}</lastmod>
    <changefreq>{page.ChangeFreq}</changefreq>
    <priority>{page.Priority}</priority>
  </url>");
            }

            sitemap.AppendLine("</urlset>");
            
            await Task.CompletedTask; // Make method truly async
            return sitemap.ToString();
        }
    }
}