using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Query.Attendance;

/// <summary>
/// Query دریافت جلسات یک دوره
/// </summary>
public class GetCourseSessionsQuery : IRequest<List<CourseSessionDto>>
{
    public Guid CourseId { get; set; }
}