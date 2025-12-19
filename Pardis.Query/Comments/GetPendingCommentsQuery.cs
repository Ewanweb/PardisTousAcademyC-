using MediatR;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Query.Comments;

public class GetPendingCommentsQuery : IRequest<List<CourseCommentDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}