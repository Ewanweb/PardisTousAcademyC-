using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pardis.Endpoints.Api.Security;

/// <summary>
/// Secure authentication service with HttpOnly cookies and CSRF protection
/// </summary>
public class SecureAuthenticationService : ISecureAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SecureAuthenticationService> _logger;
    private readonly SecureAuthOptions _options;

    public SecureAuthenticationService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<SecureAuthenticationService> logger,
        IOptions<SecureAuthOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SecureAuthResult> SignInAsync(string userId, string email, string[] roles, bool rememberMe = false)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return SecureAuthResult.Error("Invalid context");

            // Generate secure session ID
            var sessionId = GenerateSecureSessionId();
            
            // Generate CSRF token
            var csrfToken = GenerateCSRFToken();

            // Create claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Email, email),
                new("SessionId", sessionId),
                new("CSRFToken", csrfToken)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "SecureAuth");
            var principal = new ClaimsPrincipal(identity);

            // Sign in with secure cookie options
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8),
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await httpContext.SignInAsync("SecureScheme", principal, authProperties);

            // CRITICAL: Set additional security cookies
            SetSecurityCookies(httpContext, sessionId, csrfToken, rememberMe);

            // Set security headers
            SetSecurityHeaders(httpContext);

            _logger.LogInformation("Secure sign-in completed for user {UserId} from IP {IP}", 
                userId, httpContext.Connection.RemoteIpAddress);

            return SecureAuthResult.Success(csrfToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during secure sign-in for user {UserId}", userId);
            return SecureAuthResult.Error("Authentication failed");
        }
    }

    public async Task<bool> ValidateCSRFTokenAsync(string token)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return false;

            var user = httpContext.User;
            if (!user.Identity?.IsAuthenticated == true)
                return false;

            var expectedToken = user.FindFirst("CSRFToken")?.Value;
            if (string.IsNullOrEmpty(expectedToken))
                return false;

            // Constant-time comparison to prevent timing attacks
            return ConstantTimeEquals(token, expectedToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CSRF token");
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Clear authentication
            await httpContext.SignOutAsync("SecureScheme");

            // CRITICAL: Clear all security cookies
            ClearSecurityCookies(httpContext);

            _logger.LogInformation("Secure sign-out completed for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during secure sign-out");
        }
    }

    public bool IsSessionValid()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return false;

            var user = httpContext.User;
            if (!user.Identity?.IsAuthenticated == true)
                return false;

            // Validate session ID exists
            var sessionId = user.FindFirst("SessionId")?.Value;
            if (string.IsNullOrEmpty(sessionId))
                return false;

            // Additional session validation can be added here
            // e.g., check against active sessions in database

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SetSecurityCookies(HttpContext context, string sessionId, string csrfToken, bool rememberMe)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // CRITICAL: Prevent XSS access
            Secure = true,   // CRITICAL: HTTPS only
            SameSite = SameSiteMode.Strict, // CRITICAL: CSRF protection
            MaxAge = rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(8)
        };

        // Session tracking cookie
        context.Response.Cookies.Append("__Secure-SessionId", sessionId, cookieOptions);

        // CSRF token cookie (readable by JavaScript for AJAX requests)
        var csrfCookieOptions = new CookieOptions
        {
            HttpOnly = false, // Needs to be readable by JavaScript
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = cookieOptions.MaxAge
        };
        context.Response.Cookies.Append("__Secure-CSRFToken", csrfToken, csrfCookieOptions);
    }

    private void ClearSecurityCookies(HttpContext context)
    {
        var expiredCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        };

        context.Response.Cookies.Append("__Secure-SessionId", "", expiredCookieOptions);
        context.Response.Cookies.Append("__Secure-CSRFToken", "", expiredCookieOptions);
    }

    private void SetSecurityHeaders(HttpContext context)
    {
        var response = context.Response;
        
        // CRITICAL: Security headers
        response.Headers.Add("X-Content-Type-Options", "nosniff");
        response.Headers.Add("X-Frame-Options", "DENY");
        response.Headers.Add("X-XSS-Protection", "1; mode=block");
        response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'");
    }

    private string GenerateSecureSessionId()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private string GenerateCSRFToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        var result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
}

public class SecureAuthResult
{
    public bool IsSuccess { get; set; }
    public string? CSRFToken { get; set; }
    public string? ErrorMessage { get; set; }

    public static SecureAuthResult Success(string csrfToken) => 
        new() { IsSuccess = true, CSRFToken = csrfToken };

    public static SecureAuthResult Error(string message) => 
        new() { IsSuccess = false, ErrorMessage = message };
}

public class SecureAuthOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int SessionTimeoutMinutes { get; set; } = 480; // 8 hours
    public int RememberMeDays { get; set; } = 30;
}

public interface ISecureAuthenticationService
{
    Task<SecureAuthResult> SignInAsync(string userId, string email, string[] roles, bool rememberMe = false);
    Task<bool> ValidateCSRFTokenAsync(string token);
    Task SignOutAsync();
    bool IsSessionValid();
}