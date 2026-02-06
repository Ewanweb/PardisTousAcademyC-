using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Categories.UpdateCategory;

public record UpdateBlogCategoryCommand(Guid CategoryId, UpdateCategoryRequestDto Dto)
    : IRequest<OperationResult<BlogCategoryDto>>;
