using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pardis.Domain.Seo
{
    [Owned]
    public class SeoMetadata
    {
        [MaxLength(60)]
        public string? MetaTitle { get; private set; }
        
        [MaxLength(160)]
        public string? MetaDescription { get; private set; }
        
        [MaxLength(255)]
        public string? Keywords { get; private set; }
        
        [MaxLength(500)]
        public string? CanonicalUrl { get; private set; }
        
        public bool NoIndex { get; private set; }
        public bool NoFollow { get; private set; }
        
        // OpenGraph
        [MaxLength(60)]
        public string? OpenGraphTitle { get; private set; }
        
        [MaxLength(160)]
        public string? OpenGraphDescription { get; private set; }
        
        [MaxLength(500)]
        public string? OpenGraphImage { get; private set; }
        
        [MaxLength(20)]
        public string OpenGraphType { get; private set; } = "website";
        
        // Twitter Card
        [MaxLength(60)]
        public string? TwitterTitle { get; private set; }
        
        [MaxLength(160)]
        public string? TwitterDescription { get; private set; }
        
        [MaxLength(500)]
        public string? TwitterImage { get; private set; }
        
        [MaxLength(20)]
        public string TwitterCardType { get; private set; } = "summary_large_image";
        
        // JSON-LD Schemas (stored as JSON string)
        public string? JsonLdSchemas { get; private set; }

        protected SeoMetadata() { }

        public SeoMetadata(
            string? metaTitle = null,
            string? metaDescription = null,
            string? keywords = null,
            string? canonicalUrl = null,
            bool noIndex = false,
            bool noFollow = false,
            string? openGraphTitle = null,
            string? openGraphDescription = null,
            string? openGraphImage = null,
            string openGraphType = "website",
            string? twitterTitle = null,
            string? twitterDescription = null,
            string? twitterImage = null,
            string twitterCardType = "summary_large_image",
            List<object>? jsonLdSchemas = null)
        {
            MetaTitle = SanitizeAndTruncate(metaTitle, 60);
            MetaDescription = SanitizeAndTruncate(metaDescription, 160);
            Keywords = SanitizeAndTruncate(keywords, 255);
            CanonicalUrl = canonicalUrl?.Trim();
            NoIndex = noIndex;
            NoFollow = noFollow;
            OpenGraphTitle = SanitizeAndTruncate(openGraphTitle, 60);
            OpenGraphDescription = SanitizeAndTruncate(openGraphDescription, 160);
            OpenGraphImage = openGraphImage?.Trim();
            OpenGraphType = openGraphType?.Trim() ?? "website";
            TwitterTitle = SanitizeAndTruncate(twitterTitle, 60);
            TwitterDescription = SanitizeAndTruncate(twitterDescription, 160);
            TwitterImage = twitterImage?.Trim();
            TwitterCardType = twitterCardType?.Trim() ?? "summary_large_image";
            
            if (jsonLdSchemas?.Any() == true)
            {
                JsonLdSchemas = JsonSerializer.Serialize(jsonLdSchemas);
            }
        }

        public void UpdateTitle(string? title)
        {
            MetaTitle = SanitizeAndTruncate(title, 60);
        }

        public void UpdateDescription(string? description)
        {
            MetaDescription = SanitizeAndTruncate(description, 160);
        }

        public void UpdateCanonicalUrl(string? canonicalUrl)
        {
            CanonicalUrl = canonicalUrl?.Trim();
        }

        public void UpdateRobots(bool noIndex, bool noFollow)
        {
            NoIndex = noIndex;
            NoFollow = noFollow;
        }

        public void UpdateOpenGraph(string? title, string? description, string? image, string? type = null)
        {
            OpenGraphTitle = SanitizeAndTruncate(title, 60);
            OpenGraphDescription = SanitizeAndTruncate(description, 160);
            OpenGraphImage = image?.Trim();
            if (!string.IsNullOrEmpty(type))
                OpenGraphType = type.Trim();
        }

        public void UpdateTwitterCard(string? title, string? description, string? image, string? cardType = null)
        {
            TwitterTitle = SanitizeAndTruncate(title, 60);
            TwitterDescription = SanitizeAndTruncate(description, 160);
            TwitterImage = image?.Trim();
            if (!string.IsNullOrEmpty(cardType))
                TwitterCardType = cardType.Trim();
        }

        public void UpdateJsonLdSchemas(List<object>? schemas)
        {
            JsonLdSchemas = schemas?.Any() == true ? JsonSerializer.Serialize(schemas) : null;
        }

        public List<object> GetJsonLdSchemas()
        {
            if (string.IsNullOrEmpty(JsonLdSchemas))
                return new List<object>();

            try
            {
                return JsonSerializer.Deserialize<List<object>>(JsonLdSchemas) ?? new List<object>();
            }
            catch
            {
                return new List<object>();
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(MetaTitle) && 
                   string.IsNullOrEmpty(MetaDescription) && 
                   string.IsNullOrEmpty(CanonicalUrl);
        }

        public SeoValidationResult Validate()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            // Title validation
            if (string.IsNullOrEmpty(MetaTitle))
                errors.Add("SEO title is required");
            else if (MetaTitle.Length < 10)
                warnings.Add("SEO title is too short (minimum 10 characters recommended)");
            else if (MetaTitle.Length > 60)
                warnings.Add("SEO title is too long (maximum 60 characters recommended)");

            // Description validation
            if (string.IsNullOrEmpty(MetaDescription))
                errors.Add("SEO description is required");
            else if (MetaDescription.Length < 50)
                warnings.Add("SEO description is too short (minimum 50 characters recommended)");
            else if (MetaDescription.Length > 160)
                warnings.Add("SEO description is too long (maximum 160 characters recommended)");

            // Canonical URL validation
            if (!string.IsNullOrEmpty(CanonicalUrl) && !Uri.IsWellFormedUriString(CanonicalUrl, UriKind.Absolute))
                errors.Add("Canonical URL is not a valid absolute URL");

            return new SeoValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors,
                Warnings = warnings
            };
        }

        private static string? SanitizeAndTruncate(string? input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Remove HTML tags and normalize whitespace
            var sanitized = Regex.Replace(input, "<.*?>", "");
            sanitized = Regex.Replace(sanitized, @"\s+", " ").Trim();

            if (sanitized.Length <= maxLength)
                return sanitized;

            // Truncate at word boundary
            var truncated = sanitized[..(maxLength - 3)];
            var lastSpace = truncated.LastIndexOf(' ');
            
            if (lastSpace > maxLength / 2)
                return sanitized[..lastSpace] + "...";
            
            return truncated + "...";
        }
    }

    public class SeoValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
