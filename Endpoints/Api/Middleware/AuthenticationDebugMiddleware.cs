using System.Security.Claims;

namespace Api.Middleware;

/// <summary>
/// Middleware برای debug کردن مشکلات authentication
/// </summary>
public class AuthenticationDebugMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationDebugMiddleware> _logger;

    public AuthenticationDebugMiddleware(RequestDelegate next, ILogger<AuthenticationDebugMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log authentication info for debugging
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(authHeader))
            {
                _logger.LogInformation($"Request to {context.Request.Path} with Authorization header: {authHeader.Substring(0, Math.Min(30, authHeader.Length))}...");
                
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
                    var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                    
                    _logger.LogInformation($"Authenticated user: {userEmail} (ID: {userId}) with roles: [{string.Join(", ", roles)}]");
                }
                else
                {
                    _logger.LogWarning($"Authorization header present but user not authenticated for {context.Request.Path}");
                }
            }
            else if (context.Request.Path.StartsWithSegments("/api") && 
                     !context.Request.Path.StartsWithSegments("/api/auth/login") &&
                     !context.Request.Path.StartsWithSegments("/api/auth/register"))
            {
                _logger.LogWarning($"No Authorization header for protected endpoint: {context.Request.Path}");
            }
        }

        await _next(context);
        
        // Log response status for auth-related issues
        if (context.Response.StatusCode == 401)
        {
            _logger.LogWarning($"401 Unauthorized response for {context.Request.Path}");
        }
        else if (context.Response.StatusCode == 403)
        {
            _logger.LogWarning($"403 Forbidden response for {context.Request.Path}");
        }
    }
}

/// <summary>
/// Extension method برای اضافه کردن middleware
/// </summary>
public static class AuthenticationDebugMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationDebug(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationDebugMiddleware>();
    }
}