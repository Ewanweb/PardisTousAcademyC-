namespace Pardis.Domain.Dto.Attendance;

/// <summary>
/// DTO برای گزارش حضور دانشجو در یک دوره
/// </summary>
public class StudentCourseAttendanceReportDto
{
    public StudentBasicDto Student { get; set; } = new();
    public CourseBasicDto Course { get; set; } = new();
    public AttendanceSummaryDto Summary { get; set; } = new();
    public List<SessionAttendanceDetailDto> Sessions { get; set; } = new();
}

/// <summary>
/// اطلاعات پایه دانشجو
/// </summary>
public class StudentBasicDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// اطلاعات پایه دوره
/// </summary>
public class CourseBasicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// خلاصه حضور و غیاب
/// </summary>
public class AttendanceSummaryDto
{
    public int TotalSessions { get; set; }
    public int PresentSessions { get; set; }
    public int LateSessions { get; set; }
    public int AbsentSessions { get; set; }
    public double AttendanceRate { get; set; }
}

/// <summary>
/// جزئیات حضور در هر جلسه
/// </summary>
public class SessionAttendanceDetailDto
{
    public Guid SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Note { get; set; }
}