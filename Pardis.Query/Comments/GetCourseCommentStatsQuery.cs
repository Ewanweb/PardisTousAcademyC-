using MediatR;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Query.Comments;

/// <summary>
/// Query دریافت آمار کامنت‌های یک دوره
/// </summary>
public class GetCourseCommentStatsQuery : IRequest<CourseCommentStatsDto?>
{
    public Guid CourseId { get; set; }
}