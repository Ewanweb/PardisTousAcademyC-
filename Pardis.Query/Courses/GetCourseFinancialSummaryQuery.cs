using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses;

/// <summary>
/// Query دریافت خلاصه مالی یک دوره
/// </summary>
public class GetCourseFinancialSummaryQuery : IRequest<CourseFinancialSummaryDto?>
{
    public Guid CourseId { get; set; }
}