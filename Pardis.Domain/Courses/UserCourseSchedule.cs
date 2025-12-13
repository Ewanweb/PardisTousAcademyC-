using System.ComponentModel.DataAnnotations.Schema;
using Pardis.Domain.Users;

namespace Pardis.Domain.Courses;

/// <summary>
/// ثبت‌نام دانشجو در زمان‌بندی خاص دوره
/// </summary>
public class UserCourseSchedule
{
    // --- کلیدهای اصلی (رابطه) ---
    public required string UserId { get; set; }
    [ForeignKey("UserId")]
    public required User User { get; set; }

    public required Guid CourseScheduleId { get; set; }
    [ForeignKey("CourseScheduleId")]
    public required CourseSchedule CourseSchedule { get; set; }

    // --- اطلاعات ثبت‌نام ---
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// وضعیت حضور دانشجو در این زمان‌بندی
    /// </summary>
    public StudentScheduleStatus Status { get; set; } = StudentScheduleStatus.Active;

    /// <summary>
    /// تعداد جلساتی که دانشجو در این زمان‌بندی حاضر بوده
    /// </summary>
    public int AttendedSessions { get; set; } = 0;

    /// <summary>
    /// تعداد جلساتی که دانشجو در این زمان‌بندی غایب بوده
    /// </summary>
    public int AbsentSessions { get; set; } = 0;

    /// <summary>
    /// یادداشت مدرس درباره دانشجو در این زمان‌بندی
    /// </summary>
    public string? InstructorNotes { get; set; }

    public UserCourseSchedule() { }

    public UserCourseSchedule(string userId, Guid courseScheduleId)
    {
        UserId = userId;
        CourseScheduleId = courseScheduleId;
    }
}

/// <summary>
/// وضعیت دانشجو در زمان‌بندی
/// </summary>
public enum StudentScheduleStatus
{
    /// <summary>
    /// فعال - دانشجو در این زمان‌بندی شرکت می‌کند
    /// </summary>
    Active = 1,

    /// <summary>
    /// انتقال یافته - دانشجو به زمان‌بندی دیگری منتقل شده
    /// </summary>
    Transferred = 2,

    /// <summary>
    /// انصراف - دانشجو از این زمان‌بندی انصراف داده
    /// </summary>
    Withdrawn = 3,

    /// <summary>
    /// اخراج - دانشجو از این زمان‌بندی اخراج شده
    /// </summary>
    Expelled = 4
}