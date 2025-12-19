using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Query.Attendance;

/// <summary>
/// Query دریافت گزارش حضور دانشجو در یک دوره
/// </summary>
public class GetStudentCourseAttendanceQuery : IRequest<StudentCourseAttendanceReportDto?>
{
    public string StudentId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
}