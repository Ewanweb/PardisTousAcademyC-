namespace Pardis.Domain.Dto.Attendance;

/// <summary>
/// DTO برای نمایش حضور و غیاب یک جلسه
/// </summary>
public class SessionAttendanceDto
{
    public CourseSessionDto Session { get; set; } = new();
    public List<StudentAttendanceDto> Attendances { get; set; } = new();
}