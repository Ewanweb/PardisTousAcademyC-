using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Posts.CreatePost;

public record CreatePostCommand(CreatePostRequestDto Dto, string? CurrentUserId)
    : IRequest<OperationResult<PostDetailDto>>;
