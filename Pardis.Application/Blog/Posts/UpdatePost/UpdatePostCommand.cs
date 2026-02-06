using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Posts.UpdatePost;

public record UpdatePostCommand(Guid PostId, UpdatePostRequestDto Dto, string? CurrentUserId)
    : IRequest<OperationResult<PostDetailDto>>;
