namespace Pardis.Domain.Dto.Students;

public class StudentCourseAttendanceSummaryDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
    public int PresentSessions { get; set; }
    public int LateSessions { get; set; }
    public int AbsentSessions { get; set; }
    public double AttendancePercentage { get; set; }
}