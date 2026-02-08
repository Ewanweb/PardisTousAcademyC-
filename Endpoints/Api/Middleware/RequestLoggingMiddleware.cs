namespace Api.Middleware;

/// <summary>
/// میدل‌ویر لاگ کردن درخواست‌ها برای debug
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
        // لاگ درخواست‌های آپلود فایل
        if (context.Request.ContentType?.Contains("multipart/form-data") == true)
        {
            _logger.LogInformation(
                "File Upload Request | Path: {Path} | Method: {Method} | ContentType: {ContentType} | ContentLength: {ContentLength} | Origin: {Origin}",
                context.Request.Path,
                context.Request.Method,
                context.Request.ContentType,
                context.Request.ContentLength,
                context.Request.Headers["Origin"].ToString()
            );
        }

        await _next(context);

        // لاگ پاسخ
        if (context.Request.ContentType?.Contains("multipart/form-data") == true)
        {
            _logger.LogInformation(
                "File Upload Response | Path: {Path} | StatusCode: {StatusCode}",
                context.Request.Path,
                context.Response.StatusCode
            );
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
