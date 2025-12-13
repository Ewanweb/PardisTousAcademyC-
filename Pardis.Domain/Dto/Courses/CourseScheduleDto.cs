namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// DTO برای نمایش زمان‌بندی دوره
/// </summary>
public class CourseScheduleDto
{
    public required Guid Id { get; set; }
    public required Guid CourseId { get; set; }
    public required string Title { get; set; }
    public required int DayOfWeek { get; set; }
    public required string DayName { get; set; } // نام روز به فارسی
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required string TimeRange { get; set; } // مثل "12:00-14:00"
    public required string FullScheduleText { get; set; } // مثل "شنبه 12:00-14:00"
    public required int MaxCapacity { get; set; }
    public required int EnrolledCount { get; set; }
    public required int RemainingCapacity { get; set; }
    public required bool HasCapacity { get; set; }
    public required bool IsActive { get; set; }
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO برای ایجاد زمان‌بندی جدید
/// </summary>
public class CreateCourseScheduleDto
{
    public required Guid CourseId { get; set; }
    public required string Title { get; set; }
    public required int DayOfWeek { get; set; } // 0=یکشنبه, 6=شنبه
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required int MaxCapacity { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO برای ویرایش زمان‌بندی
/// </summary>
public class UpdateCourseScheduleDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required int DayOfWeek { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required int MaxCapacity { get; set; }
    public required bool IsActive { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO برای ثبت‌نام در زمان‌بندی
/// </summary>
public class EnrollInScheduleDto
{
    public required Guid CourseScheduleId { get; set; }
}

/// <summary>
/// DTO برای نمایش دانشجویان یک زمان‌بندی
/// </summary>
public class ScheduleStudentDto
{
    public required string UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Mobile { get; set; }
    public required DateTime EnrolledAt { get; set; }
    public required string Status { get; set; }
    public required int AttendedSessions { get; set; }
    public required int AbsentSessions { get; set; }
    public string? InstructorNotes { get; set; }
}