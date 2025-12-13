using System.ComponentModel.DataAnnotations.Schema;
using Pardis.Domain.Users;

namespace Pardis.Domain.Courses;

public class UserCourse
{
    // --- کلیدهای اصلی (رابطه) ---
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public Guid CourseId { get; set; }
    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    // --- اطلاعات مالی و ثبت‌نام ---
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public long PurchasePrice { get; set; } // مبلغ پرداخت شده

    /// <summary>
    /// وضعیت دانشجو در این دوره (فعال، انصرافی، اخراج شده و...)
    /// </summary>
    public StudentCourseStatus Status { get; set; } = StudentCourseStatus.Active;

    // --- مدیریت کلاس‌های حضوری و لایو (LMS Features) ---

    /// <summary>
    /// تعداد جلساتی که دانشجو حاضر بوده است
    /// </summary>
    public int AttendedSessionsCount { get; set; } = 0;

    /// <summary>
    /// تعداد جلساتی که دانشجو غیبت داشته است
    /// </summary>
    public int AbsentSessionsCount { get; set; } = 0;

    /// <summary>
    /// شماره صندلی یا ردیف (مخصوص دوره‌های حضوری)
    /// </summary>
    public string? SeatNumber { get; set; }

    /// <summary>
    /// لینک اختصاصی ورود به کلاس آنلاین (اگر برای هر کاربر متفاوت باشد)
    /// </summary>
    public string? ExclusiveLiveLink { get; set; }

    /// <summary>
    /// آیا دسترسی ورود به کلاس (لایو یا فیزیکی) مسدود شده؟ (مثلاً به دلیل بدهی)
    /// </summary>
    public bool IsAccessBlocked { get; set; } = false;

    // --- نتایج و گواهینامه ---

    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// نمره نهایی دانشجو (0 تا 100)
    /// </summary>
    public int? FinalGrade { get; set; }

    /// <summary>
    /// کد یکتای گواهینامه (Certificate ID)
    /// </summary>
    public string? CertificateCode { get; set; }

    /// <summary>
    /// یادداشت محرمانه مدرس درباره این دانشجو
    /// </summary>
    public string? InstructorNotes { get; set; }
}