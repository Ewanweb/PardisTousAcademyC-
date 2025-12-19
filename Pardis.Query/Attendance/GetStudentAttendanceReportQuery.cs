using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Query.Attendance;

public class GetStudentAttendanceReportQuery : IRequest<StudentAttendanceReportDto>
{
    public string StudentId { get; set; }
    public Guid CourseId { get; set; }

    public GetStudentAttendanceReportQuery(string studentId, Guid courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
    }
}
