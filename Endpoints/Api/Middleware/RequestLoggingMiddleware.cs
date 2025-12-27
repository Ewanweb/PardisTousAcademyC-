using System.Diagnostics;
using System.Text;

namespace Api.Middleware
{
    /// <summary>
    /// Middleware برای لاگ کردن درخواست‌ها و پاسخ‌ها
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            // Add request ID to response headers
            context.Response.Headers["X-Request-ID"] = requestId;
            
            // Log request
            await LogRequest(context, requestId);
            
            // Capture response
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred. RequestId: {RequestId}", requestId);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                // Log response
                await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);
                
                // Copy response back to original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogRequest(HttpContext context, string requestId)
        {
            var request = context.Request;
            
            var logData = new
            {
                RequestId = requestId,
                Method = request.Method,
                Path = request.Path.Value,
                QueryString = request.QueryString.Value,
                Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                UserAgent = request.Headers.UserAgent.ToString(),
                RemoteIP = context.Connection.RemoteIpAddress?.ToString(),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("HTTP Request: {RequestData}", 
                System.Text.Json.JsonSerializer.Serialize(logData));

            // Log request body for POST/PUT requests (be careful with sensitive data)
            if (request.Method == "POST" || request.Method == "PUT")
            {
                if (request.ContentType?.Contains("application/json") == true)
                {
                    request.EnableBuffering();
                    var body = await new StreamReader(request.Body).ReadToEndAsync();
                    request.Body.Position = 0;
                    
                    if (!string.IsNullOrEmpty(body))
                    {
                        _logger.LogDebug("Request Body for {RequestId}: {Body}", requestId, body);
                    }
                }
            }
        }

        private async Task LogResponse(HttpContext context, string requestId, long elapsedMs)
        {
            var response = context.Response;
            
            var logData = new
            {
                RequestId = requestId,
                StatusCode = response.StatusCode,
                ContentType = response.ContentType,
                ContentLength = response.ContentLength,
                ElapsedMilliseconds = elapsedMs,
                Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Timestamp = DateTime.UtcNow
            };

            var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            _logger.Log(logLevel, "HTTP Response: {ResponseData}", 
                System.Text.Json.JsonSerializer.Serialize(logData));

            // Log response body for errors (in development)
            if (response.StatusCode >= 400 && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
                
                if (!string.IsNullOrEmpty(responseBody))
                {
                    _logger.LogDebug("Error Response Body for {RequestId}: {Body}", requestId, responseBody);
                }
            }
        }
    }

    /// <summary>
    /// Extension method برای اضافه کردن middleware
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}