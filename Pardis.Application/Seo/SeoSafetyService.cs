using Pardis.Domain.Seo;
using Microsoft.Extensions.Logging;

namespace Pardis.Application.Seo
{
    public interface ISeoSafetyService
    {
        SeoSafetyResult ValidateAndSanitize(SeoMeta seoMeta);
        SeoMeta GenerateFallbackSeo(SeoEntityType entityType, string title, string description);
        bool IsSlugSafe(string slug);
        string SanitizeSlug(string slug);
        SeoMeta HandleMissingSeoData(SeoEntityType entityType, Dictionary<string, object> entityContext);
    }

    public class SeoSafetyResult
    {
        public bool IsValid { get; set; }
        public SeoMeta SanitizedSeo { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Changes { get; set; } = new();
    }

    public class SeoMetaValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class SeoMeta
    {
        public string Title { get; }
        public string Description { get; }
        public string? Keywords { get; }
        public string? CanonicalUrl { get; }
        public bool RobotsIndex { get; }
        public bool RobotsFollow { get; }
        public string? OpenGraphTitle { get; }
        public string? OpenGraphDescription { get; }
        public string? OpenGraphImage { get; }
        public string OpenGraphType { get; }
        public string? TwitterTitle { get; }
        public string? TwitterDescription { get; }
        public string? TwitterImage { get; }
        public string TwitterCardType { get; }

        public SeoMeta(
            string title = "",
            string description = "",
            string? keywords = null,
            string? canonicalUrl = null,
            bool robotsIndex = true,
            bool robotsFollow = true,
            string? openGraphTitle = null,
            string? openGraphDescription = null,
            string? openGraphImage = null,
            string openGraphType = "website",
            string? twitterTitle = null,
            string? twitterDescription = null,
            string? twitterImage = null,
            string twitterCardType = "summary_large_image")
        {
            Title = title;
            Description = description;
            Keywords = keywords;
            CanonicalUrl = canonicalUrl;
            RobotsIndex = robotsIndex;
            RobotsFollow = robotsFollow;
            OpenGraphTitle = openGraphTitle;
            OpenGraphDescription = openGraphDescription;
            OpenGraphImage = openGraphImage;
            OpenGraphType = openGraphType;
            TwitterTitle = twitterTitle;
            TwitterDescription = twitterDescription;
            TwitterImage = twitterImage;
            TwitterCardType = twitterCardType;
        }

        public SeoMetaValidationResult Validate()
        {
            var result = new SeoMetaValidationResult();

            if (string.IsNullOrWhiteSpace(Title))
                result.Errors.Add("Title is required");
            else if (Title.Length < 10)
                result.Warnings.Add("Title is shorter than recommended");

            if (string.IsNullOrWhiteSpace(Description))
                result.Errors.Add("Description is required");
            else if (Description.Length < 50)
                result.Warnings.Add("Description is shorter than recommended");

            result.IsValid = result.Errors.Count == 0;
            return result;
        }
    }

    public class SeoSafetyService : ISeoSafetyService
    {
        private readonly ILogger<SeoSafetyService> _logger;
        private readonly string _brandName = "آکادمی پردیس توس";

        public SeoSafetyService(ILogger<SeoSafetyService> logger)
        {
            _logger = logger;
        }

        public SeoSafetyResult ValidateAndSanitize(SeoMeta seoMeta)
        {
            var result = new SeoSafetyResult();
            var changes = new List<string>();

            // Sanitize title
            var originalTitle = seoMeta.Title;
            var sanitizedTitle = SanitizeTitle(seoMeta.Title);
            if (sanitizedTitle != originalTitle)
            {
                changes.Add($"Title sanitized: '{originalTitle}' -> '{sanitizedTitle}'");
            }

            // Sanitize description
            var originalDescription = seoMeta.Description;
            var sanitizedDescription = SanitizeDescription(seoMeta.Description);
            if (sanitizedDescription != originalDescription)
            {
                changes.Add($"Description sanitized");
            }

            // Sanitize canonical URL
            var originalCanonical = seoMeta.CanonicalUrl;
            var sanitizedCanonical = SanitizeCanonicalUrl(seoMeta.CanonicalUrl);
            if (sanitizedCanonical != originalCanonical)
            {
                changes.Add($"Canonical URL sanitized: '{originalCanonical}' -> '{sanitizedCanonical}'");
            }

            // Create sanitized SEO meta
            result.SanitizedSeo = new SeoMeta(
                title: sanitizedTitle,
                description: sanitizedDescription,
                keywords: SanitizeKeywords(seoMeta.Keywords),
                canonicalUrl: sanitizedCanonical,
                robotsIndex: seoMeta.RobotsIndex,
                robotsFollow: seoMeta.RobotsFollow,
                openGraphTitle: SanitizeTitle(seoMeta.OpenGraphTitle ?? sanitizedTitle),
                openGraphDescription: SanitizeDescription(seoMeta.OpenGraphDescription ?? sanitizedDescription),
                openGraphImage: SanitizeImageUrl(seoMeta.OpenGraphImage),
                openGraphType: seoMeta.OpenGraphType,
                twitterTitle: SanitizeTitle(seoMeta.TwitterTitle ?? sanitizedTitle),
                twitterDescription: SanitizeDescription(seoMeta.TwitterDescription ?? sanitizedDescription),
                twitterImage: SanitizeImageUrl(seoMeta.TwitterImage),
                twitterCardType: seoMeta.TwitterCardType
            );

            // Validate the sanitized result
            var validation = result.SanitizedSeo.Validate();
            result.Errors = validation.Errors;
            result.Warnings = validation.Warnings;
            result.Changes = changes;
            result.IsValid = validation.IsValid;

            if (changes.Any())
            {
                _logger.LogWarning("SEO data sanitized with {ChangeCount} changes: {Changes}", 
                    changes.Count, string.Join("; ", changes));
            }

            return result;
        }

        public SeoMeta GenerateFallbackSeo(SeoEntityType entityType, string title, string description)
        {
            var fallbackTitle = GenerateFallbackTitle(entityType, title);
            var fallbackDescription = GenerateFallbackDescription(entityType, description);

            return new SeoMeta(
                title: fallbackTitle,
                description: fallbackDescription,
                robotsIndex: true,
                robotsFollow: true,
                openGraphTitle: fallbackTitle,
                openGraphDescription: fallbackDescription,
                openGraphType: GetOpenGraphType(entityType),
                twitterTitle: fallbackTitle,
                twitterDescription: fallbackDescription,
                twitterCardType: "summary_large_image"
            );
        }

        public bool IsSlugSafe(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            // Check for dangerous patterns
            var dangerousPatterns = new[]
            {
                "..", "//", "\\", "<", ">", "\"", "'", "&", "%", "?", "#"
            };

            return !dangerousPatterns.Any(pattern => slug.Contains(pattern));
        }

        public string SanitizeSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Guid.NewGuid().ToString("N")[..8];

            // Remove dangerous characters
            var sanitized = slug
                .Replace("..", "")
                .Replace("//", "/")
                .Replace("\\", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("\"", "")
                .Replace("'", "")
                .Replace("&", "")
                .Replace("%", "")
                .Replace("?", "")
                .Replace("#", "");

            // Ensure it's not empty after sanitization
            if (string.IsNullOrWhiteSpace(sanitized))
                return Guid.NewGuid().ToString("N")[..8];

            return sanitized;
        }

        public SeoMeta HandleMissingSeoData(SeoEntityType entityType, Dictionary<string, object> entityContext)
        {
            var title = entityContext.TryGetValue("title", out var titleObj) ? titleObj?.ToString() : "";
            var description = entityContext.TryGetValue("description", out var descObj) ? descObj?.ToString() : "";

            if (string.IsNullOrEmpty(title))
            {
                title = GenerateDefaultTitle(entityType);
                _logger.LogWarning("Missing title for {EntityType}, using fallback: {Title}", entityType, title);
            }

            if (string.IsNullOrEmpty(description))
            {
                description = GenerateDefaultDescription(entityType);
                _logger.LogWarning("Missing description for {EntityType}, using fallback", entityType);
            }

            return GenerateFallbackSeo(entityType, title, description);
        }

        private string SanitizeTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return _brandName;

            // Remove HTML tags
            var sanitized = System.Text.RegularExpressions.Regex.Replace(title, "<.*?>", "");
            
            // Remove excessive whitespace
            sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"\s+", " ").Trim();
            
            // Limit length
            if (sanitized.Length > 60)
            {
                sanitized = sanitized.Substring(0, 57) + "...";
            }

            // Ensure minimum length
            if (sanitized.Length < 10)
            {
                sanitized = $"{sanitized} | {_brandName}";
            }

            return sanitized;
        }

        private string SanitizeDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "دوره‌های پروژه‌محور برنامه‌نویسی و طراحی وب با پشتیبانی منتور";

            // Remove HTML tags
            var sanitized = System.Text.RegularExpressions.Regex.Replace(description, "<.*?>", "");
            
            // Remove excessive whitespace
            sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"\s+", " ").Trim();
            
            // Limit length
            if (sanitized.Length > 160)
            {
                sanitized = sanitized.Substring(0, 157) + "...";
            }

            // Ensure minimum length
            if (sanitized.Length < 50)
            {
                sanitized = $"{sanitized} - آموزش با کیفیت در {_brandName}";
            }

            return sanitized;
        }

        private string? SanitizeCanonicalUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            // Basic URL validation
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return null;

            // Ensure HTTPS
            if (uri.Scheme != "https")
            {
                var builder = new UriBuilder(uri) { Scheme = "https", Port = -1 };
                return builder.ToString();
            }

            return url;
        }

        private string? SanitizeKeywords(string? keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return null;

            // Remove HTML and excessive whitespace
            var sanitized = System.Text.RegularExpressions.Regex.Replace(keywords, "<.*?>", "");
            sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"\s+", " ").Trim();

            // Limit length
            if (sanitized.Length > 255)
            {
                sanitized = sanitized.Substring(0, 252) + "...";
            }

            return sanitized;
        }

        private string? SanitizeImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            // Basic URL validation
            if (!Uri.TryCreate(imageUrl, UriKind.RelativeOrAbsolute, out var uri))
                return null;

            return imageUrl;
        }

        private string GenerateFallbackTitle(SeoEntityType entityType, string title)
        {
            var sanitizedTitle = SanitizeTitle(title);
            
            return entityType switch
            {
                SeoEntityType.Course => $"{sanitizedTitle} | دوره آموزشی",
                SeoEntityType.Category => $"دوره‌های {sanitizedTitle}",
                SeoEntityType.Page => sanitizedTitle,
                SeoEntityType.Home => _brandName,
                _ => sanitizedTitle
            };
        }

        private string GenerateFallbackDescription(SeoEntityType entityType, string description)
        {
            var sanitizedDescription = SanitizeDescription(description);
            
            return entityType switch
            {
                SeoEntityType.Course => $"{sanitizedDescription} - آموزش با پروژه‌های واقعی در {_brandName}",
                SeoEntityType.Category => $"{sanitizedDescription} - مجموعه دوره‌های تخصصی",
                SeoEntityType.Page => sanitizedDescription,
                SeoEntityType.Home => "دوره‌های پروژه‌محور برنامه‌نویسی، طراحی وب و مهارت‌های دیجیتال",
                _ => sanitizedDescription
            };
        }

        private string GenerateDefaultTitle(SeoEntityType entityType)
        {
            return entityType switch
            {
                SeoEntityType.Course => "دوره آموزشی",
                SeoEntityType.Category => "دسته‌بندی دوره‌ها",
                SeoEntityType.Page => "صفحه",
                SeoEntityType.Home => _brandName,
                _ => _brandName
            };
        }

        private string GenerateDefaultDescription(SeoEntityType entityType)
        {
            return entityType switch
            {
                SeoEntityType.Course => "دوره آموزشی با پروژه‌های واقعی و پشتیبانی منتور",
                SeoEntityType.Category => "مجموعه دوره‌های تخصصی با آموزش قدم‌به‌قدم",
                SeoEntityType.Page => "اطلاعات مفید در آکادمی پردیس توس",
                SeoEntityType.Home => "دوره‌های پروژه‌محور برنامه‌نویسی و طراحی وب",
                _ => "آکادمی پردیس توس"
            };
        }

        private string GetOpenGraphType(SeoEntityType entityType)
        {
            return entityType switch
            {
                SeoEntityType.Course => "article",
                SeoEntityType.Category => "website",
                SeoEntityType.Page => "website",
                SeoEntityType.Home => "website",
                _ => "website"
            };
        }
    }
}
