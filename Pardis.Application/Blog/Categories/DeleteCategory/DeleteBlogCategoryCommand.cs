using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Blog.Categories.DeleteCategory;

public record DeleteBlogCategoryCommand(Guid CategoryId) : IRequest<OperationResult>;
