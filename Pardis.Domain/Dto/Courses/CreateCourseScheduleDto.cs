namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// DTO ایجاد زمان‌بندی دوره
/// </summary>
public class CreateCourseScheduleDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO بروزرسانی زمان‌بندی دوره
/// </summary>
public class UpdateCourseScheduleDto
{
    public string Title { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}