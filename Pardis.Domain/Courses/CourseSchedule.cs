using System.ComponentModel.DataAnnotations.Schema;

namespace Pardis.Domain.Courses;

/// <summary>
/// زمان‌های برگذاری دوره - مثل شنبه 12-14 یا چهارشنبه 18-20
/// </summary>
public class CourseSchedule : BaseEntity
{
    public required Guid CourseId { get; set; }
    [ForeignKey("CourseId")]
    public required Course Course { get; set; }

    /// <summary>
    /// نام زمان‌بندی - مثل "گروه صبح" یا "گروه عصر"
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// روز هفته - 0=یکشنبه, 1=دوشنبه, ..., 6=شنبه
    /// </summary>
    public required int DayOfWeek { get; set; }

    /// <summary>
    /// ساعت شروع - مثل "12:00"
    /// </summary>
    public required TimeOnly StartTime { get; set; }

    /// <summary>
    /// ساعت پایان - مثل "14:00"
    /// </summary>
    public required TimeOnly EndTime { get; set; }

    /// <summary>
    /// حداکثر ظرفیت دانشجو
    /// </summary>
    public required int MaxCapacity { get; set; }

    /// <summary>
    /// تعداد دانشجویان ثبت‌نام شده
    /// </summary>
    public int EnrolledCount { get; set; } = 0;

    /// <summary>
    /// آیا این زمان‌بندی فعال است؟
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// توضیحات اضافی - مثل "کلاس حضوری" یا "آنلاین"
    /// </summary>
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<UserCourseSchedule> StudentEnrollments { get; set; } = [];

    public CourseSchedule() { }

    public CourseSchedule(Guid courseId, string title, int dayOfWeek, TimeOnly startTime, TimeOnly endTime, int maxCapacity, string? description = null)
    {
        CourseId = courseId;
        Title = title;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        MaxCapacity = maxCapacity;
        Description = description;
    }

    /// <summary>
    /// نام روز هفته به فارسی
    /// </summary>
    public string GetDayName()
    {
        return DayOfWeek switch
        {
            0 => "یکشنبه",
            1 => "دوشنبه", 
            2 => "سه‌شنبه",
            3 => "چهارشنبه",
            4 => "پنج‌شنبه",
            5 => "جمعه",
            6 => "شنبه",
            _ => "نامشخص"
        };
    }

    /// <summary>
    /// متن کامل زمان‌بندی - مثل "شنبه 12:00-14:00"
    /// </summary>
    public string GetFullScheduleText()
    {
        return $"{GetDayName()} {StartTime:HH:mm}-{EndTime:HH:mm}";
    }

    /// <summary>
    /// آیا ظرفیت باقی مانده دارد؟
    /// </summary>
    public bool HasCapacity => EnrolledCount < MaxCapacity;

    /// <summary>
    /// ظرفیت باقی مانده
    /// </summary>
    public int RemainingCapacity => Math.Max(0, MaxCapacity - EnrolledCount);
}