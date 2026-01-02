using MediatR;

namespace Pardis.Query.Courses.GetCourseStudents;

/// <summary>
/// Query for getting course students
/// </summary>
public class GetCourseStudentsQuery : IRequest<GetCourseStudentsResult>
{
    public Guid CourseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Sort { get; set; }
}

/// <summary>
/// Result for course students query
/// </summary>
public class GetCourseStudentsResult
{
    public CourseInfoDto Course { get; set; } = new();
    public CourseStatsDto Stats { get; set; } = new();
    public List<CourseStudentDto> Items { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>
/// Course basic info DTO
/// </summary>
public class CourseInfoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// Course statistics DTO
/// </summary>
public class CourseStatsDto
{
    public int TotalStudents { get; set; }
    public int Active { get; set; }
    public int Completed { get; set; }
    public int Dropped { get; set; }
    public decimal AvgProgress { get; set; }
}

/// <summary>
/// Course student DTO
/// </summary>
public class CourseStudentDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ProgressPercent { get; set; }
    public DateTime? LastSeenAt { get; set; }
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