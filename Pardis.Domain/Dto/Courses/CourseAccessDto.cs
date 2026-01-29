namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// اطلاعات دسترسی به دوره - فقط برای کاربران خریدار
/// </summary>
public class CourseAccessDto
{
    /// <summary>
    /// آدرس محل برگزاری کلاس (برای دوره‌های حضوری و ترکیبی)
    /// </summary>
    public string? ClassLocation { get; set; }

    /// <summary>
    /// لینک ورود به کلاس آنلاین (برای دوره‌های آنلاین و ترکیبی)
    /// </summary>
    public string? LiveUrl { get; set; }

    /// <summary>
    /// آیا دسترسی کاربر به این دوره مسدود شده است؟
    /// </summary>
    public bool IsAccessBlocked { get; set; }

    /// <summary>
    /// شماره صندلی یا ردیف (برای دوره‌های حضوری)
    /// </summary>
    public string? SeatNumber { get; set; }

    /// <summary>
    /// تاریخ ثبت‌نام کاربر در این دوره
    /// </summary>
    public DateTime EnrolledAt { get; set; }

    /// <summary>
    /// آیا کاربر این دوره را تکمیل کرده است؟
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// تاریخ تکمیل دوره (در صورت تکمیل)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// نمره نهایی کاربر (0 تا 100)
    /// </summary>
    public int? FinalGrade { get; set; }

    /// <summary>
    /// کد گواهینامه (در صورت تکمیل موفق)
    /// </summary>
    public string? CertificateCode { get; set; }
}