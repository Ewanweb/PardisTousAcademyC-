using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.GetMyCourses;

/// <summary>
/// Query for getting instructor's courses
/// </summary>
public class GetMyCoursesQuery : IRequest<GetMyCoursesResult>
{
    public string UserId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Sort { get; set; }
}

/// <summary>
/// Result for my courses query
/// </summary>
public class GetMyCoursesResult
{
    public List<MyCourseDto> Items { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>
/// DTO for instructor's course
/// </summary>
public class MyCourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int StudentsCount { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// Pagination DTO
/// </summary>
public class PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}