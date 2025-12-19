using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Application.Comments;

/// <summary>
/// Command ایجاد کامنت دوره
/// </summary>
public class CreateCommentCommand : IRequest<OperationResult<CourseCommentDto>>
{
    public Guid CourseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}