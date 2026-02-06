using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Tags.CreateTag;

public record CreateTagCommand(CreateTagRequestDto Dto) : IRequest<OperationResult<TagDto>>;
