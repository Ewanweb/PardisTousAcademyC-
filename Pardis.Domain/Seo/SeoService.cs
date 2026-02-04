using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Pardis.Domain.Seo
{
    public interface ISeoService
    {
        string GenerateSlug(string title, string? existingSlug = null);
        Task<string> GenerateUniqueSlugAsync<T>(string title, Func<string, Task<bool>> slugExistsCheck, string? existingSlug = null);
        SeoMetadata GenerateDefaultSeo(string title, string description, string slug, SeoType seoType);
        bool ValidateSeoData(SeoMetadata seo, out List<string> errors);
        string OptimizeMetaDescription(string description, int maxLength = 160);
        string BuildCanonicalUrl(string path, Dictionary<string, string>? parameters = null);
    }

    public enum SeoType
    {
        Course,
        Category,
        BlogPost,
        Page
    }

    public class SeoService : ISeoService
    {
        private readonly string _baseUrl;
        private static readonly Regex SlugRegex = new(@"[^a-zA-Z0-9\u0600-\u06FF\u200C\u200D\-_]", RegexOptions.Compiled);
        
        public SeoService(IConfiguration configuration)
        {
            _baseUrl = configuration["SiteSettings:BaseUrl"] ?? "https://pardistous.ir";
        }

        public string GenerateSlug(string title, string? existingSlug = null)
        {
            if (!string.IsNullOrEmpty(existingSlug))
                return existingSlug;

            if (string.IsNullOrWhiteSpace(title))
                return Guid.NewGuid().ToString("N")[..8];

            // Persian and English slug generation
            var slug = title.Trim()
                .Replace(" ", "-")
                .Replace("_", "-")
                .Replace(".", "-")
                .Replace("/", "-")
                .Replace("\\", "-");

            // Remove invalid characters but keep Persian characters
            slug = SlugRegex.Replace(slug, "");
            
            // Remove multiple consecutive dashes
            slug = Regex.Replace(slug, "-+", "-");
            
            // Remove leading/trailing dashes
            slug = slug.Trim('-');
            
            // Ensure minimum length
            if (slug.Length < 3)
                slug = $"{slug}-{Guid.NewGuid().ToString("N")[..4]}";

            return slug.ToLowerInvariant();
        }

        public async Task<string> GenerateUniqueSlugAsync<T>(
            string title, 
            Func<string, Task<bool>> slugExistsCheck, 
            string? existingSlug = null)
        {
            var baseSlug = GenerateSlug(title, existingSlug);
            var slug = baseSlug;
            var counter = 1;

            while (await slugExistsCheck(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
                
                // Prevent infinite loops
                if (counter > 100)
                {
                    slug = $"{baseSlug}-{Guid.NewGuid().ToString("N")[..8]}";
                    break;
                }
            }

            return slug;
        }

        public SeoMetadata GenerateDefaultSeo(string title, string description, string slug, SeoType seoType)
        {
            var seo = new SeoMetadata(
                metaTitle: OptimizeTitle(title, seoType),
                metaDescription: OptimizeMetaDescription(description),
                canonicalUrl: BuildCanonicalUrl(GetPathForType(seoType, slug)),
                noIndex: false,
                noFollow: false
            );

            return seo;
        }

        public bool ValidateSeoData(SeoMetadata seo, out List<string> errors)
        {
            errors = new List<string>();

            // Title validation
            if (string.IsNullOrWhiteSpace(seo.MetaTitle))
                errors.Add("Meta title is required");
            else if (seo.MetaTitle.Length < 10)
                errors.Add("Meta title too short (minimum 10 characters)");
            else if (seo.MetaTitle.Length > 60)
                errors.Add("Meta title too long (maximum 60 characters)");

            // Description validation
            if (string.IsNullOrWhiteSpace(seo.MetaDescription))
                errors.Add("Meta description is required");
            else if (seo.MetaDescription.Length < 50)
                errors.Add("Meta description too short (minimum 50 characters)");
            else if (seo.MetaDescription.Length > 160)
                errors.Add("Meta description too long (maximum 160 characters)");

            // Canonical URL validation
            if (!string.IsNullOrEmpty(seo.CanonicalUrl) && !Uri.IsWellFormedUriString(seo.CanonicalUrl, UriKind.Absolute))
                errors.Add("Invalid canonical URL format");

            return errors.Count == 0;
        }

        public string OptimizeMetaDescription(string description, int maxLength = 160)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;

            description = description.Trim();
            
            if (description.Length <= maxLength)
                return description;

            // Find last complete sentence within limit
            var truncated = description[..(maxLength - 3)];
            var lastSentenceEnd = Math.Max(
                truncated.LastIndexOf('.'),
                Math.Max(truncated.LastIndexOf('!'), truncated.LastIndexOf('?'))
            );

            if (lastSentenceEnd > maxLength / 2)
                return description[..(lastSentenceEnd + 1)];

            // Find last complete word
            var lastSpace = truncated.LastIndexOf(' ');
            if (lastSpace > maxLength / 2)
                return description[..lastSpace] + "...";

            return truncated + "...";
        }

        public string BuildCanonicalUrl(string path, Dictionary<string, string>? parameters = null)
        {
            var baseUrl = _baseUrl.TrimEnd('/');
            var normalizedPath = path.StartsWith('/') ? path : $"/{path}";
            
            if (parameters == null || parameters.Count == 0)
                return $"{baseUrl}{normalizedPath}";

            var queryString = string.Join("&", 
                parameters.Where(p => !string.IsNullOrEmpty(p.Value))
                         .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

            return $"{baseUrl}{normalizedPath}?{queryString}";
        }

        private string OptimizeTitle(string title, SeoType seoType)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "آکادمی پردیس توس";

            var optimized = title.Trim();
            
            // Add context based on type
            var suffix = seoType switch
            {
                SeoType.Course => " | دوره آموزشی",
                SeoType.Category => " | دسته‌بندی دوره‌ها",
                SeoType.BlogPost => " | وبلاگ",
                _ => ""
            };

            var maxTitleLength = 60 - suffix.Length - " | آکادمی پردیس توس".Length;
            
            if (optimized.Length > maxTitleLength)
                optimized = optimized[..(maxTitleLength - 3)] + "...";

            return $"{optimized}{suffix}";
        }

        private string GetPathForType(SeoType seoType, string slug)
        {
            return seoType switch
            {
                SeoType.Course => $"/course/{slug}",
                SeoType.Category => $"/category/{slug}",
                SeoType.BlogPost => $"/blog/{slug}",
                _ => $"/{slug}"
            };
        }
    }
}