using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses;

/// <summary>
/// Query دریافت دانشجویان یک دوره
/// </summary>
public class GetCourseStudentsQuery : IRequest<List<CourseStudentDto>>
{
    public Guid CourseId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}