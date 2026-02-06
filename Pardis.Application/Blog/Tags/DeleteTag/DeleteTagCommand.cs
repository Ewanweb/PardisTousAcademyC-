using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Blog.Tags.DeleteTag;

public record DeleteTagCommand(Guid TagId) : IRequest<OperationResult>;
