using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Tags.UpdateTag;

public record UpdateTagCommand(Guid TagId, UpdateTagRequestDto Dto) : IRequest<OperationResult<TagDto>>;
