using MediatR;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Query.Comments;

public class GetCourseCommentsQuery : IRequest<List<CourseCommentDto>>
{
    public Guid CourseId { get; set; }
    public CommentStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeRejected { get; set; } = false;
}