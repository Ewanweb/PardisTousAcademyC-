namespace Pardis.Domain.Dto;

/// <summary>
/// پاسخ استاندارد API
/// </summary>
/// <typeparam name="T">نوع داده</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// وضعیت موفقیت عملیات
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// پیام نتیجه عملیات
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// داده‌های بازگشتی
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// کد خطا (در صورت وجود)
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// جزئیات خطا (برای debugging)
    /// </summary>
    public object? ErrorDetails { get; set; }

    /// <summary>
    /// زمان پاسخ
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ایجاد پاسخ موفق با داده
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, string message = "عملیات با موفقیت انجام شد")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// ایجاد پاسخ موفق بدون داده
    /// </summary>
    public static ApiResponse<T> SuccessResult(string message = "عملیات با موفقیت انجام شد")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// ایجاد پاسخ خطا
    /// </summary>
    public static ApiResponse<T> ErrorResult(string message, string? errorCode = null, object? errorDetails = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ErrorDetails = errorDetails
        };
    }

    /// <summary>
    /// ایجاد پاسخ یافت نشد
    /// </summary>
    public static ApiResponse<T> NotFoundResult(string message = "اطلاعات درخواستی یافت نشد")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = "NOT_FOUND"
        };
    }

    /// <summary>
    /// ایجاد پاسخ عدم دسترسی
    /// </summary>
    public static ApiResponse<T> UnauthorizedResult(string message = "دسترسی غیرمجاز")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = "UNAUTHORIZED"
        };
    }

    /// <summary>
    /// ایجاد پاسخ اعتبارسنجی نامعتبر
    /// </summary>
    public static ApiResponse<T> ValidationErrorResult(string message, object? validationErrors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = "VALIDATION_ERROR",
            ErrorDetails = validationErrors
        };
    }
}

/// <summary>
/// پاسخ استاندارد API بدون داده
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// ایجاد پاسخ موفق بدون داده
    /// </summary>
    public static new ApiResponse SuccessResult(string message = "عملیات با موفقیت انجام شد")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// ایجاد پاسخ خطا
    /// </summary>
    public static new ApiResponse ErrorResult(string message, string? errorCode = null, object? errorDetails = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ErrorDetails = errorDetails
        };
    }

    /// <summary>
    /// ایجاد پاسخ یافت نشد
    /// </summary>
    public static new ApiResponse NotFoundResult(string message = "اطلاعات درخواستی یافت نشد")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = "NOT_FOUND"
        };
    }

    /// <summary>
    /// ایجاد پاسخ عدم دسترسی
    /// </summary>
    public static new ApiResponse UnauthorizedResult(string message = "دسترسی غیرمجاز")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = "UNAUTHORIZED"
        };
    }

    /// <summary>
    /// ایجاد پاسخ اعتبارسنجی نامعتبر
    /// </summary>
    public static new ApiResponse ValidationErrorResult(string message, object? validationErrors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = "VALIDATION_ERROR",
            ErrorDetails = validationErrors
        };
    }
}