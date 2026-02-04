using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Pardis.Application.Seo
{
    public interface ICanonicalUrlService
    {
        string NormalizeUrl(string url);
        string BuildCanonicalUrl(string path, Dictionary<string, string>? queryParams = null);
        bool ShouldIndex(string path, Dictionary<string, string>? queryParams = null);
        string GetRobotsDirective(string path, Dictionary<string, string>? queryParams = null);
    }

    public class CanonicalUrlService : ICanonicalUrlService
    {
        private readonly string _baseUrl;
        private readonly HashSet<string> _allowedParams;
        private readonly Dictionary<string, int> _maxPageDepth;

        public CanonicalUrlService(IConfiguration configuration)
        {
            _baseUrl = configuration["SiteSettings:BaseUrl"]?.TrimEnd('/') ?? "https://pardistous.ir";
            
            // Parameters that are allowed in canonical URLs
            _allowedParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "page", "category", "level", "price", "sort"
            };

            // Maximum page depth for indexing
            _maxPageDepth = new Dictionary<string, int>
            {
                { "category", 10 },
                { "course", 1 },
                { "search", 5 }
            };
        }

        public string NormalizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            // Parse URL
            var uri = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var parsedUri) ? parsedUri : null;
            if (uri == null)
                return url;

            // Make absolute if relative
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(new Uri(_baseUrl), uri);
            }

            var builder = new UriBuilder(uri);
            
            // Remove fragment
            builder.Fragment = string.Empty;
            
            // Normalize path (remove trailing slash except for root)
            if (builder.Path.Length > 1 && builder.Path.EndsWith('/'))
            {
                builder.Path = builder.Path.TrimEnd('/');
            }

            // Filter and sort query parameters
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var filteredParams = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string key in queryParams.Keys)
            {
                if (key != null && _allowedParams.Contains(key))
                {
                    var value = queryParams[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        filteredParams[key] = value;
                    }
                }
            }

            // Rebuild query string
            if (filteredParams.Any())
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

        public string BuildCanonicalUrl(string path, Dictionary<string, string>? queryParams = null)
        {
            var normalizedPath = path.StartsWith('/') ? path : $"/{path}";
            
            // Remove trailing slash except for root
            if (normalizedPath.Length > 1 && normalizedPath.EndsWith('/'))
            {
                normalizedPath = normalizedPath.TrimEnd('/');
            }

            var url = $"{_baseUrl}{normalizedPath}";

            if (queryParams != null && queryParams.Any())
            {
                var filteredParams = queryParams
                    .Where(p => _allowedParams.Contains(p.Key) && !string.IsNullOrEmpty(p.Value))
                    .OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (filteredParams.Any())
                {
                    var queryString = string.Join("&", 
                        filteredParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                    url = $"{url}?{queryString}";
                }
            }

            return url;
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
                var pathType = GetPathType(path);
                if (_maxPageDepth.TryGetValue(pathType, out var maxDepth) && page > maxDepth)
                {
                    return false;
                }
            }

            // Don't index complex filter combinations
            var filterParams = queryParams?.Where(p => p.Key != "page" && _allowedParams.Contains(p.Key)).Count() ?? 0;
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

        private string GetPathType(string path)
        {
            if (path.StartsWith("/category/", StringComparison.OrdinalIgnoreCase))
                return "category";
            if (path.StartsWith("/course/", StringComparison.OrdinalIgnoreCase))
                return "course";
            if (path.StartsWith("/search", StringComparison.OrdinalIgnoreCase))
                return "search";
            
            return "page";
        }
    }

    // Middleware to handle canonical redirects
    public class CanonicalRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICanonicalUrlService _canonicalUrlService;

        public CanonicalRedirectMiddleware(RequestDelegate next, ICanonicalUrlService canonicalUrlService)
        {
            _next = next;
            _canonicalUrlService = canonicalUrlService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var currentUrl = $"{request.Path}{request.QueryString}";
            var normalizedUrl = _canonicalUrlService.NormalizeUrl(currentUrl);

            // If URLs don't match, redirect to canonical version
            if (!string.Equals(currentUrl, normalizedUrl.Replace(request.Scheme + "://" + request.Host, ""), StringComparison.OrdinalIgnoreCase))
            {
                var canonicalPath = normalizedUrl.Replace(request.Scheme + "://" + request.Host, "");
                context.Response.Redirect(canonicalPath, permanent: true);
                return;
            }

            await _next(context);
        }
    }

    // Extension method for middleware registration
    public static class CanonicalRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseCanonicalRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CanonicalRedirectMiddleware>();
        }
    }
}
