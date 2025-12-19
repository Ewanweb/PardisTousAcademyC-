using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses;

/// <summary>
/// Query دریافت زمان‌بندی‌های دوره
/// </summary>
public class GetCourseSchedulesQuery : IRequest<List<CourseScheduleDto>>
{
    public Guid CourseId { get; set; }
}