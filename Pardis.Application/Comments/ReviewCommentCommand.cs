using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Application.Comments;

/// <summary>
/// Command بررسی کامنت توسط ادمین
/// </summary>
public class ReviewCommentCommand : IRequest<OperationResult<CourseCommentDto>>
{
    public Guid CommentId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public CommentStatus Status { get; set; }
    public string? Note { get; set; }
}