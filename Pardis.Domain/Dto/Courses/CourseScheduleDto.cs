namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// DTO زمان‌بندی دوره
/// </summary>
public class CourseScheduleDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
    public string FullScheduleText { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int EnrolledCount { get; set; }
    public int RemainingCapacity { get; set; }
    public bool HasCapacity { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO دانشجوی زمان‌بندی
/// </summary>
public class ScheduleStudentDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AttendedSessions { get; set; }
    public int AbsentSessions { get; set; }
    public string? InstructorNotes { get; set; }
}