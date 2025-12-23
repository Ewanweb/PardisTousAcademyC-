using Microsoft.AspNetCore.Mvc;
using MediatR;
using Pardis.Application._Shared;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// کنترلر پایه با مدیریت خطاهای یکسان
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected readonly ILogger _logger;

    protected BaseController(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        _logger = logger;
    }

    // Constructor برای کنترلرهای قدیمی که فقط ILogger دارن
    protected BaseController(ILogger logger)
    {
        _logger = logger;
        Mediator = null!; // برای کنترلرهای قدیمی که MediatR استفاده نمی‌کنن
    }

    /// <summary>
    /// دریافت شناسه کاربر فعلی
    /// </summary>
    protected string? GetCurrentUserId()
    {
        return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// ایجاد پاسخ بر اساس OperationResult
    /// </summary>
    protected IActionResult CreateResponse(OperationResult result)
    {
        return result.Status switch
        {
            OperationResultStatus.Success => Ok(new { success = true, message = result.Message }),
            OperationResultStatus.NotFound => NotFound(new { success = false, message = result.Message }),
            OperationResultStatus.Error => BadRequest(new { success = false, message = result.Message }),
            _ => StatusCode(500, new { success = false, message = "خطای غیرمنتظره" })
        };
    }

    /// <summary>
    /// ایجاد پاسخ بر اساس OperationResult با داده
    /// </summary>
    protected IActionResult CreateResponse<T>(OperationResult<T> result)
    {
        return result.Status switch
        {
            OperationResultStatus.Success => Ok(new { success = true, message = result.Message, data = result.Data }),
            OperationResultStatus.NotFound => NotFound(new { success = false, message = result.Message }),
            OperationResultStatus.Error => BadRequest(new { success = false, message = result.Message }),
            _ => StatusCode(500, new { success = false, message = "خطای غیرمنتظره" })
        };
    }

    /// <summary>
    /// مدیریت نتیجه عملیات و بازگشت پاسخ مناسب
    /// </summary>
    protected IActionResult HandleOperationResult<T>(OperationResult<T> result, string? successMessage = null)
    {
        try
        {
            return result.Status switch
            {
                OperationResultStatus.Success => Ok(new
                {
                    success = true,
                    message = successMessage ?? "عملیات با موفقیت انجام شد",
                    data = result.Data
                }),
                OperationResultStatus.NotFound => NotFound(new
                {
                    success = false,
                    message = result.Message ?? "اطلاعات درخواستی یافت نشد"
                }),
                OperationResultStatus.Error => BadRequest(new
                {
                    success = false,
                    message = result.Message ?? "خطا در انجام عملیات"
                }),
                _ => StatusCode(500, new
                {
                    success = false,
                    message = "خطای غیرمنتظره‌ای رخ داد"
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در مدیریت نتیجه عملیات: {Message}", ex.Message);
            return StatusCode(500, new
            {
                success = false,
                message = "خطای غیرمنتظره‌ای رخ داد"
            });
        }
    }

    /// <summary>
    /// مدیریت نتیجه عملیات بدون داده
    /// </summary>
    protected IActionResult HandleOperationResult(OperationResult result, string? successMessage = null)
    {
        try
        {
            return result.Status switch
            {
                OperationResultStatus.Success => Ok(new
                {
                    success = true,
                    message = successMessage ?? result.Message ?? "عملیات با موفقیت انجام شد"
                }),
                OperationResultStatus.NotFound => NotFound(new
                {
                    success = false,
                    message = result.Message ?? "اطلاعات درخواستی یافت نشد"
                }),
                OperationResultStatus.Error => BadRequest(new
                {
                    success = false,
                    message = result.Message ?? "خطا در انجام عملیات"
                }),
                _ => StatusCode(500, new
                {
                    success = false,
                    message = "خطای غیرمنتظره‌ای رخ داد"
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در مدیریت نتیجه عملیات: {Message}", ex.Message);
            return StatusCode(500, new
            {
                success = false,
                message = "خطای غیرمنتظره‌ای رخ داد"
            });
        }
    }

    /// <summary>
    /// بازگشت پاسخ موفق با داده
    /// </summary>
    protected IActionResult SuccessResponse<T>(T data, string? message = null)
    {
        return Ok(new
        {
            success = true,
            message = message ?? "عملیات با موفقیت انجام شد",
            data,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// بازگشت پاسخ موفق بدون داده
    /// </summary>
    protected IActionResult SuccessResponse(string? message = null)
    {
        return Ok(new
        {
            success = true,
            message = message ?? "عملیات با موفقیت انجام شد",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// بازگشت پاسخ خطا
    /// </summary>
    protected IActionResult ErrorResponse(string message, int statusCode = 400, string? errorCode = null, object? errorDetails = null)
    {
        return StatusCode(statusCode, new
        {
            success = false,
            message,
            errorCode,
            errorDetails,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// بازگشت پاسخ یافت نشد
    /// </summary>
    protected IActionResult NotFoundResponse(string message = "اطلاعات درخواستی یافت نشد")
    {
        return NotFound(new
        {
            success = false,
            message,
            errorCode = "NOT_FOUND",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// بازگشت پاسخ عدم دسترسی
    /// </summary>
    protected IActionResult UnauthorizedResponse(string message = "دسترسی غیرمجاز")
    {
        return Unauthorized(new
        {
            success = false,
            message,
            errorCode = "UNAUTHORIZED",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// بازگشت پاسخ اعتبارسنجی نامعتبر
    /// </summary>
    protected IActionResult ValidationErrorResponse(string message, object? validationErrors = null)
    {
        return BadRequest(new
        {
            success = false,
            message,
            errorCode = "VALIDATION_ERROR",
            errorDetails = validationErrors,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// اجرای عملیات با مدیریت خطا
    /// </summary>
    protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> operation, string? errorMessage = null)
    {
        try
        {
            return await operation();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "دسترسی غیرمجاز: {Message}", ex.Message);
            return Unauthorized(new
            {
                success = false,
                message = "دسترسی غیرمجاز. لطفاً وارد حساب کاربری خود شوید"
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "پارامتر نامعتبر: {Message}", ex.Message);
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای غیرمنتظره: {Message}", ex.Message);
            return StatusCode(500, new
            {
                success = false,
                message = errorMessage ?? "خطای غیرمنتظره‌ای رخ داد. لطفاً دوباره تلاش کنید"
            });
        }
    }
}