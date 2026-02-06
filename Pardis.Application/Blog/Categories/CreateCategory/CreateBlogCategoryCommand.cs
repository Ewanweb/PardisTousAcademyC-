using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Categories.CreateCategory;

public record CreateBlogCategoryCommand(CreateCategoryRequestDto Dto, string? CurrentUserId)
    : IRequest<OperationResult<BlogCategoryDto>>;
