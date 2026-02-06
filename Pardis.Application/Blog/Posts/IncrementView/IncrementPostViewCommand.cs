using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Blog.Posts.IncrementView;

public record IncrementPostViewCommand(string Slug) : IRequest<OperationResult>;
