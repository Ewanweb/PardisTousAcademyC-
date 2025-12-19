using System.Net;
using System.Text.Json;
using Pardis.Application._Shared.Exceptions;

namespace Api.Middleware;

/// <summary>
/// میدل‌ویر مدیریت خطاهای سراسری
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // لاگ کامل خطا با جزئیات درخواست
            _logger.LogError(ex, 
                "خطای غیرمنتظره رخ داد | Path: {Path} | Method: {Method} | Query: {Query} | Message: {Message} | StackTrace: {StackTrace}",
                context.Request.Path,
                context.Request.Method,
                context.Request.QueryString,
                ex.Message,
                ex.StackTrace);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        object response;
        
        if (exception is ValidationException validationEx)
        {
            response = new
            {
                success = false,
                message = GetErrorMessage(exception),
                errors = validationEx.Errors.Any() 
                    ? validationEx.Errors 
                    : new Dictionary<string, string[]> { { "general", new[] { validationEx.Message } } }
            };
        }
        else
        {
            var isDevelopment = context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true;
            
            response = new
            {
                success = false,
                message = GetErrorMessage(exception),
                error = new
                {
                    type = exception.GetType().Name,
                    details = isDevelopment ? exception.Message : null,
                    stackTrace = isDevelopment ? exception.StackTrace : null,
                    innerException = isDevelopment && exception.InnerException != null 
                        ? new { message = exception.InnerException.Message, type = exception.InnerException.GetType().Name }
                        : null
                }
            };
        }

        context.Response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            BusinessException businessEx => businessEx.UserFriendlyMessage,
            ValidationException => "اطلاعات ارسالی نامعتبر است",
            UnauthorizedAccessException => "دسترسی غیرمجاز. لطفاً وارد حساب کاربری خود شوید",
            ArgumentNullException => "اطلاعات ارسالی ناقص است",
            ArgumentException => "اطلاعات ارسالی نامعتبر است",
            InvalidOperationException => "عملیات درخواستی امکان‌پذیر نیست",
            TimeoutException => "زمان انتظار تمام شد. لطفاً دوباره تلاش کنید",
            HttpRequestException => "خطا در ارتباط با سرور. لطفاً اتصال اینترنت خود را بررسی کنید",
            _ => "خطای غیرمنتظره‌ای رخ داد. لطفاً دوباره تلاش کنید"
        };
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BusinessException => (int)HttpStatusCode.BadRequest,
            ValidationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            HttpRequestException => (int)HttpStatusCode.ServiceUnavailable,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
}