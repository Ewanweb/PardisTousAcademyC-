using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Application.Seo.Resolvers;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Pages;
using Pardis.Domain.Seo;
using System.Collections;
using System.Text.RegularExpressions;

namespace Pardis.Application.Seo
{
    public class SeoService : ISeoService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _baseUrl;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(1);
        private readonly HashSet<string> _allowedParams = new() { "page", "category", "level", "price", "sort" };

        private static readonly Regex SlugRegex = new(@"[^a-zA-Z0-9\u0600-\u06FF\u200C\u200D\-_]", RegexOptions.Compiled);

        public SeoService(
            IMemoryCache cache,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _cache = cache;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _baseUrl = _configuration["SiteSettings:BaseUrl"]?.TrimEnd('/') ?? "https://pardistous.ir";
        }

        public async Task<SeoDto> ResolveSeoAsync(ISeoEntity entity, SeoContext context)
        {
            var cacheKey = $"seo:{entity.GetSeoEntityType()}:{entity.Slug}:{context.Language}:{context.Page}";
            
            if (_cache.TryGetValue(cacheKey, out SeoDto? cachedSeo) && cachedSeo != null)
            {
                return cachedSeo;
            }

            var seoDto = entity.GetSeoEntityType() switch
            {
                SeoEntityType.Category => await _serviceProvider.GetRequiredService<CategorySeoResolver>()
                    .ResolveAsync((Category)entity, context),
                SeoEntityType.Course => await _serviceProvider.GetRequiredService<CourseSeoResolver>()
                    .ResolveAsync((Course)entity, context),
                SeoEntityType.Page => await _serviceProvider.GetRequiredService<PageSeoResolver>()
                    .ResolveAsync((Page)entity, context),
                _ => throw new ArgumentException($"No resolver found for entity type: {entity.GetSeoEntityType()}")
            };
            
            _cache.Set(cacheKey, seoDto, _cacheExpiry);
            return seoDto;
        }

        public async Task<SeoDto> ResolveSeoBySlugAsync(SeoEntityType entityType, string slug, SeoContext context)
        {
            var cacheKey = $"seo:{entityType}:{slug}:{context.Language}:{context.Page}";
            
            if (_cache.TryGetValue(cacheKey, out SeoDto? cachedSeo) && cachedSeo != null)
            {
                return cachedSeo;
            }

            var resolver = GetResolverBase(entityType);
            var seoDto = await resolver.ResolveBySlugAsync(slug, context);
            
            _cache.Set(cacheKey, seoDto, _cacheExpiry);
            return seoDto;
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

        public async Task<string> GenerateUniqueSlugAsync(string title, SeoEntityType entityType, string? existingSlug = null)
        {
            var baseSlug = GenerateSlug(title, existingSlug);
            var slug = baseSlug;
            var counter = 1;

            while (await SlugExistsAsync(slug, entityType))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
                
                if (counter > 100)
                {
                    slug = $"{baseSlug}-{Guid.NewGuid().ToString("N")[..8]}";
                    break;
                }
            }

            return slug;
        }

        public string BuildCanonicalUrl(string path, Dictionary<string, string>? queryParams = null, string? baseUrl = null)
        {
            var url = (baseUrl ?? _baseUrl).TrimEnd('/');
            var normalizedPath = path.StartsWith('/') ? path : $"/{path}";
            
            // Remove trailing slash except for root
            if (normalizedPath.Length > 1 && normalizedPath.EndsWith('/'))
            {
                normalizedPath = normalizedPath.TrimEnd('/');
            }

            if (queryParams == null || queryParams.Count == 0)
                return $"{url}{normalizedPath}";

            var filteredParams = queryParams
                .Where(p => _allowedParams.Contains(p.Key.ToLower()) && !string.IsNullOrEmpty(p.Value))
                .OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (filteredParams.Count == 0)
                return $"{url}{normalizedPath}";

            var queryString = string.Join("&", 
                filteredParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            
            return $"{url}{normalizedPath}?{queryString}";
        }

        public string NormalizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var uri = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var parsedUri) ? parsedUri : null;
            if (uri == null)
                return url;

            // Make absolute if relative
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(new Uri(_baseUrl), uri);
            }

            var builder = new UriBuilder(uri)
            {
                Fragment = string.Empty
            };
            
            // Normalize path
            if (builder.Path.Length > 1 && builder.Path.EndsWith('/'))
            {
                builder.Path = builder.Path.TrimEnd('/');
            }

            // Filter query parameters
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var filteredParams = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string key in queryParams.Keys)
            {
                if (key != null && _allowedParams.Contains(key.ToLower()))
                {
                    var value = queryParams[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        filteredParams[key] = value;
                    }
                }
            }

            if (filteredParams.Count > 0)
            {
                var queryString = string.Join("&", 
                    filteredParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                builder.Query = queryString;
            }
            else
            {
                builder.Query = string.Empty;
            }

            return builder.ToString();
        }

        public bool ShouldIndex(string path, Dictionary<string, string>? queryParams = null)
        {
            // Don't index admin, profile, or private pages
            var noIndexPaths = new[] { "/admin", "/profile", "/cart", "/orders", "/checkout", "/login", "/register" };
            if (noIndexPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // Check pagination depth
            if (queryParams?.TryGetValue("page", out var pageStr) == true && int.TryParse(pageStr, out var page))
            {
                if (page > 10) // Don't index deep pagination
                {
                    return false;
                }
            }

            // Don't index complex filter combinations
            var filterParams = queryParams?.Where(p => !string.Equals(p.Key, "page", StringComparison.OrdinalIgnoreCase) && _allowedParams.Contains(p.Key.ToLower())).Count() ?? 0;
            if (filterParams > 2)
            {
                return false;
            }

            return true;
        }

        public string GetRobotsDirective(string path, Dictionary<string, string>? queryParams = null)
        {
            var shouldIndex = ShouldIndex(path, queryParams);
            var index = shouldIndex ? "index" : "noindex";
            var follow = "follow"; // Generally allow following links

            // Special cases
            if (path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
            {
                return "noindex, nofollow, noarchive, nosnippet";
            }

            return $"{index}, {follow}";
        }

        public async Task InvalidateCacheAsync(SeoEntityType entityType, string slug)
        {
            // Simple cache invalidation - in production, use pattern-based invalidation with Redis
            if (_cache is MemoryCache memoryCache)
            {
                var field = typeof(MemoryCache).GetField("_coherentState", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field?.GetValue(memoryCache) is object coherentState)
                {
                    var entriesCollection = coherentState.GetType()
                        .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (entriesCollection?.GetValue(coherentState) is IDictionary entries)
                    {
                        var keysToRemove = new List<object>();
                        foreach (DictionaryEntry entry in entries)
                        {
                            if (entry.Key.ToString()?.Contains($"seo:{entityType}:{slug}") == true)
                            {
                                keysToRemove.Add(entry.Key);
                            }
                        }
                        
                        foreach (var key in keysToRemove)
                        {
                            _cache.Remove(key);
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }

        private ISeoResolverBase GetResolverBase(SeoEntityType entityType)
        {
            return entityType switch
            {
                SeoEntityType.Category => _serviceProvider.GetRequiredService<CategorySeoResolver>(),
                SeoEntityType.Course => _serviceProvider.GetRequiredService<CourseSeoResolver>(),
                SeoEntityType.Page => _serviceProvider.GetRequiredService<PageSeoResolver>(),
                _ => throw new ArgumentException($"No resolver found for entity type: {entityType}")
            };
        }

        private static async Task<bool> SlugExistsAsync(string slug, SeoEntityType entityType)
        {
            // This would check the database for existing slugs
            // Implementation depends on your repository pattern
            // For now, return false as placeholder
            await Task.CompletedTask;
            return false;
        }
    }
}
