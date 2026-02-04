using Microsoft.Extensions.DependencyInjection;
using Pardis.Application.Seo;
using Pardis.Domain.Seo;

namespace Api.Middleware
{
    public class SlugRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public SlugRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            
            if (ShouldCheckForRedirect(path))
            {
                var redirectService = context.RequestServices.GetRequiredService<ISlugRedirectService>();
                var entityType = GetEntityTypeFromPath(path);
                var slug = GetSlugFromPath(path);

                if (entityType.HasValue && !string.IsNullOrEmpty(slug))
                {
                    var newSlug = await redirectService.GetRedirectUrlAsync(slug, entityType.Value);
                    if (!string.IsNullOrEmpty(newSlug))
                    {
                        var newPath = path.Replace(slug, newSlug);
                        var redirectUrl = $"{newPath}{context.Request.QueryString}";
                        
                        context.Response.StatusCode = 301; // Permanent redirect
                        context.Response.Headers.Location = redirectUrl;
                        return;
                    }
                }
            }

            await _next(context);
        }

        private static bool ShouldCheckForRedirect(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            
            return path.StartsWith("/course/") || 
                   path.StartsWith("/category/") || 
                   path.StartsWith("/page/");
        }

        private static SeoEntityType? GetEntityTypeFromPath(string path)
        {
            if (path.StartsWith("/course/")) return SeoEntityType.Course;
            if (path.StartsWith("/category/")) return SeoEntityType.Category;
            if (path.StartsWith("/page/")) return SeoEntityType.Page;
            return null;
        }

        private static string GetSlugFromPath(string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments.Length >= 2 ? segments[1] : string.Empty;
        }
    }
}