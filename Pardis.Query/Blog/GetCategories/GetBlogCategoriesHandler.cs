using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Seo;

namespace Pardis.Query.Blog.GetCategories;

public class GetBlogCategoriesHandler : IRequestHandler<GetBlogCategoriesQuery, List<BlogCategoryDto>>
{
    private readonly IRepository<BlogCategory> _categoryRepository;

    public GetBlogCategoriesHandler(IRepository<BlogCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<BlogCategoryDto>> Handle(GetBlogCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _categoryRepository.Table.AsNoTracking().Where(c => !c.IsDeleted);

        return await query
            .OrderByDescending(c => c.Priority)
            .ThenBy(c => c.Title)
            .Select(c => new BlogCategoryDto
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                Description = c.Description,
                Priority = c.Priority,
                Seo = new SeoDto
                {
                    MetaTitle = c.SeoMetadata.MetaTitle,
                    MetaDescription = c.SeoMetadata.MetaDescription,
                    CanonicalUrl = c.SeoMetadata.CanonicalUrl,
                    NoIndex = c.SeoMetadata.NoIndex,
                    NoFollow = c.SeoMetadata.NoFollow
                }
            })
            .ToListAsync(cancellationToken);
    }
}
