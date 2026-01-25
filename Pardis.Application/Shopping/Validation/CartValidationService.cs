using Pardis.Application.Courses.Contracts;
using Pardis.Application.Payments.Contracts;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Shopping;
using Pardis.Domain.Courses;

namespace Pardis.Application.Shopping.Validation;

/// <summary>
/// سرویس اعتبارسنجی سبد خرید - ساده شده
/// </summary>
public interface ICartValidationService
{
    Task<CartValidationResult> ValidateAddCourseToCartAsync(string userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<CartValidationResult> ValidateCartForCheckoutAsync(string userId, CancellationToken cancellationToken = default);
    Task<CartValidationResult> ValidateCourseDataIntegrityAsync(Domain.Shopping.Cart cart, CancellationToken cancellationToken = default);
}

public class CartValidationService : ICartValidationService
{
    private readonly Application.Courses.Contracts.ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICartRepository _cartRepository;

    public CartValidationService(
        Application.Courses.Contracts.ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ICartRepository cartRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _cartRepository = cartRepository;
    }

    public async Task<CartValidationResult> ValidateAddCourseToCartAsync(string userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var result = new CartValidationResult();

        // 1. بررسی وجود دوره
        var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course == null)
        {
            result.AddError("COURSE_NOT_FOUND", "دوره مورد نظر یافت نشد");
            return result;
        }

        // 2. بررسی فعال بودن دوره
        if (course.Status != CourseStatus.Published)
        {
            result.AddError("COURSE_INACTIVE", "این دوره در حال حاضر غیرفعال است");
            return result;
        }

        // 3. بررسی ثبت‌نام قبلی
        var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId, cancellationToken);
        if (existingEnrollment != null)
        {
            result.AddError("ALREADY_ENROLLED", "شما قبلاً در این دوره ثبت‌نام کرده‌اید");
            return result;
        }

        // 4. بررسی وجود در سبد خرید
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart != null && cart.ContainsCourse(courseId))
        {
            result.AddError("ALREADY_IN_CART", "این دوره قبلاً به سبد خرید اضافه شده است");
            return result;
        }

        result.IsValid = true;
        result.Course = course;
        return result;
    }

    public async Task<CartValidationResult> ValidateCartForCheckoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = new CartValidationResult();

        // 1. بررسی وجود سبد خرید
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart == null || cart.IsEmpty())
        {
            result.AddError("CART_EMPTY", "سبد خرید خالی است");
            return result;
        }

        // 2. بررسی انقضای سبد
        if (cart.IsExpired())
        {
            result.AddError("CART_EXPIRED", "سبد خرید منقضی شده است. لطفاً دوره‌ها را مجدداً اضافه کنید");
            return result;
        }

        // 3. بررسی صحت شناسه سبد
        if (cart.Id == Guid.Empty)
        {
            result.AddError("INVALID_CART_ID", "شناسه سبد خرید معتبر نیست");
            return result;
        }

        // 4. بررسی یکپارچگی داده‌های دوره‌ها
        var integrityCheck = await ValidateCourseDataIntegrityAsync(cart, cancellationToken);
        if (!integrityCheck.IsValid)
        {
            result.AddErrors(integrityCheck.Errors);
            return result;
        }

        // 5. بررسی مجدد ثبت‌نام‌ها
        foreach (var item in cart.Items)
        {
            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, item.CourseId, cancellationToken);
            if (enrollment != null)
            {
                result.AddError("ENROLLMENT_CONFLICT", $"شما در حین خرید در دوره '{item.TitleSnapshot}' ثبت‌نام کرده‌اید");
                return result;
            }
        }

        result.IsValid = true;
        result.Cart = cart;
        return result;
    }

    public async Task<CartValidationResult> ValidateCourseDataIntegrityAsync(Domain.Shopping.Cart cart, CancellationToken cancellationToken = default)
    {
        var result = new CartValidationResult();
        var hasChanges = false;

        foreach (var item in cart.Items)
        {
            var currentCourse = await _courseRepository.GetByIdAsync(item.CourseId, cancellationToken);
            
            // بررسی وجود دوره
            if (currentCourse == null)
            {
                result.AddError("COURSE_DELETED", $"دوره '{item.TitleSnapshot}' حذف شده است");
                continue;
            }

            // بررسی فعال بودن دوره
            if (currentCourse.Status != CourseStatus.Published)
            {
                result.AddError("COURSE_DEACTIVATED", $"دوره '{item.TitleSnapshot}' غیرفعال شده است");
                continue;
            }

            // بررسی تغییر قیمت
            if (currentCourse.Price != item.UnitPrice)
            {
                result.AddWarning("PRICE_CHANGED", 
                    $"قیمت دوره '{item.TitleSnapshot}' از {item.UnitPrice:N0} به {currentCourse.Price:N0} تومان تغییر کرده است");
                hasChanges = true;
            }

            // بررسی تغییر عنوان
            if (currentCourse.Title != item.TitleSnapshot)
            {
                result.AddWarning("TITLE_CHANGED", 
                    $"عنوان دوره از '{item.TitleSnapshot}' به '{currentCourse.Title}' تغییر کرده است");
                hasChanges = true;
            }

            // بررسی تغییر مدرس (اگر اطلاعات مدرس در دسترس باشد)
            if (currentCourse.Instructor != null && currentCourse.Instructor.FullName != item.InstructorSnapshot)
            {
                result.AddWarning("INSTRUCTOR_CHANGED", 
                    $"مدرس دوره '{item.TitleSnapshot}' تغییر کرده است");
                hasChanges = true;
            }
        }

        if (hasChanges && result.Errors.Count == 0)
        {
            result.AddInfo("DATA_UPDATED", "برخی اطلاعات دوره‌ها به‌روزرسانی شده است");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}

/// <summary>
/// نتیجه اعتبارسنجی سبد خرید
/// </summary>
public class CartValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationError> Warnings { get; set; } = new();
    public List<ValidationError> Infos { get; set; } = new();
    
    // اطلاعات اضافی
    public Course? Course { get; set; }
    public Domain.Shopping.Cart? Cart { get; set; }

    public void AddError(string code, string message)
    {
        Errors.Add(new ValidationError(code, message, ValidationSeverity.Error));
    }

    public void AddWarning(string code, string message)
    {
        Warnings.Add(new ValidationError(code, message, ValidationSeverity.Warning));
    }

    public void AddInfo(string code, string message)
    {
        Infos.Add(new ValidationError(code, message, ValidationSeverity.Info));
    }

    public void AddErrors(IEnumerable<ValidationError> errors)
    {
        Errors.AddRange(errors);
    }

    public bool HasErrors => Errors.Any();
    public bool HasWarnings => Warnings.Any();
    public bool HasInfos => Infos.Any();
}

/// <summary>
/// خطای اعتبارسنجی
/// </summary>
public class ValidationError
{
    public string Code { get; set; }
    public string Message { get; set; }
    public ValidationSeverity Severity { get; set; }

    public ValidationError(string code, string message, ValidationSeverity severity)
    {
        Code = code;
        Message = message;
        Severity = severity;
    }
}

/// <summary>
/// سطح شدت خطا
/// </summary>
public enum ValidationSeverity
{
    Info,
    Warning,
    Error
}