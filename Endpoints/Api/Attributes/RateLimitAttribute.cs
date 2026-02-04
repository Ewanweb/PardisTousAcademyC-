using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Api.Attributes;

/// <summary>
/// Rate limiting attribute for API endpoints
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly int _maxRequests;
    private readonly int _windowMinutes;
    private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    /// <summary>
    /// Initialize rate limit attribute
    /// </summary>
    /// <param name="maxRequests">Maximum number of requests allowed</param>
    /// <param name="windowMinutes">Time window in minutes</param>
    public int MaxRequests { get; set; }
    public int WindowMinutes { get; set; }

    public RateLimitAttribute(int maxRequests = 100, int windowMinutes = 1)
    {
        _maxRequests = maxRequests;
        _windowMinutes = windowMinutes;
        MaxRequests = maxRequests;
        WindowMinutes = windowMinutes;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var key = GenerateClientKey(context);
        var requestCounts = GetRequestCounts(key);
        
        if (requestCounts >= _maxRequests)
        {
            context.Result = new ContentResult
            {
                StatusCode = (int)HttpStatusCode.TooManyRequests,
                Content = "Rate limit exceeded. Please try again later."
            };
            return;
        }

        IncrementRequestCount(key);
        base.OnActionExecuting(context);
    }

    private string GenerateClientKey(ActionExecutingContext context)
    {
        var clientId = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userId = context.HttpContext.User?.Identity?.Name ?? "anonymous";
        var endpoint = $"{context.ActionDescriptor.RouteValues["controller"]}.{context.ActionDescriptor.RouteValues["action"]}";
        
        return $"rate_limit:{clientId}:{userId}:{endpoint}";
    }

    private int GetRequestCounts(string key)
    {
        return _cache.Get<int>(key);
    }

    private void IncrementRequestCount(string key)
    {
        var currentCount = _cache.Get<int>(key);
        var newCount = currentCount + 1;
        
        var expiry = TimeSpan.FromMinutes(_windowMinutes);
        _cache.Set(key, newCount, expiry);
    }
}
