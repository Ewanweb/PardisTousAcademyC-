using MediatR;
using Pardis.Domain.Dto.Students;

namespace Pardis.Query.Students;

/// <summary>
/// Query دریافت خلاصه حضور و غیاب دانشجو
/// </summary>
public class GetStudentAttendanceSummaryQuery : IRequest<List<StudentCourseAttendanceSummaryDto>>
{
    public string StudentId { get; set; } = string.Empty;
}