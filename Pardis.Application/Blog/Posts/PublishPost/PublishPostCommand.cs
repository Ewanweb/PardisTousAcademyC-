using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Blog.Posts.PublishPost;

public record PublishPostCommand(Guid PostId, DateTime? PublishedAt = null) : IRequest<OperationResult>;
