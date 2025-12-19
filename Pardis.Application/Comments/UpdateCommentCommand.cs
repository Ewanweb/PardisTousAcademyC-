using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Application.Comments;

/// <summary>
/// Command بروزرسانی کامنت
/// </summary>
public class UpdateCommentCommand : IRequest<OperationResult<CourseCommentDto>>
{
    public Guid CommentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}