using MediatR;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetCategories;

public class GetBlogCategoriesQuery : IRequest<List<BlogCategoryDto>>
{
    public bool IncludeEmpty { get; set; } = true;
}
