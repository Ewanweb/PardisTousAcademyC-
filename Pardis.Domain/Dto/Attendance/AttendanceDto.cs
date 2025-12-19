using Pardis.Domain.Attendance;

namespace Pardis.Domain.Dto.Attendance;

/// <summary>
/// DTO جلسه دوره
/// </summary>
public class CourseSessionDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid? ScheduleId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? ScheduleTitle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan Duration { get; set; }
    public int SessionNumber { get; set; }
    public SessionStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PresentStudents { get; set; }
    public int AbsentStudents { get; set; }
    public int LateStudents { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CourseStudentDto> Students { get; set; } = new();
}

/// <summary>
/// DTO دانشجوی دوره برای حضور و غیاب
/// </summary>
public class CourseStudentDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? StudentEmail { get; set; }
    public string? StudentMobile { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public AttendanceStatus? CurrentSessionStatus { get; set; }
    public string? CurrentSessionStatusDisplay { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO ایجاد جلسه
/// </summary>
public class CreateSessionDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan Duration { get; set; }
    public int SessionNumber { get; set; }
}

/// <summary>
/// DTO بروزرسانی جلسه
/// </summary>
public class UpdateSessionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// DTO حضور و غیاب دانشجو
/// </summary>
public class StudentAttendanceDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public AttendanceStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public TimeSpan? AttendanceDuration { get; set; }
    public string? Note { get; set; }
    public string? RecordedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO ثبت حضور و غیاب
/// </summary>
public class RecordAttendanceDto
{
    public string StudentId { get; set; } = string.Empty;
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO گزارش حضور و غیاب دانشجو
/// </summary>
public class StudentAttendanceReportDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int PresentSessions { get; set; }
    public int AbsentSessions { get; set; }
    public int LateSessions { get; set; }
    public decimal AttendancePercentage { get; set; }
    public List<StudentAttendanceDto> AttendanceDetails { get; set; } = new();
}