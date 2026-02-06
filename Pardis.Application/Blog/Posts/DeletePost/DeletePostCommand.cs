using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Blog.Posts.DeletePost;

public record DeletePostCommand(Guid PostId) : IRequest<OperationResult>;
