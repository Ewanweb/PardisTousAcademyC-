using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostsByCategory;

public class GetPostsByCategorySlugHandler : IRequestHandler<GetPostsByCategorySlugQuery, PagedResult<PostListItemDto>>
{
    private readonly IRepository<Post> _postRepository;

    public GetPostsByCategorySlugHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PagedResult<PostListItemDto>> Handle(GetPostsByCategorySlugQuery request, CancellationToken cancellationToken)
    {
        var pagination = PaginationHelper.Normalize(new PaginationRequest { Page = request.Page, PageSize = request.PageSize });

        var query = _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published && p.BlogCategory.Slug == request.Slug)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        pagination = PaginationHelper.ClampPage(pagination, total);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new PostListItemDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                CoverImageUrl = p.CoverImageUrl,
                Author = p.Author,
                CategoryTitle = p.BlogCategory.Title,
                CategorySlug = p.BlogCategory.Slug,
                PublishedAt = p.PublishedAt,
                ReadingTimeMinutes = p.ReadingTimeMinutes,
                ViewCount = p.Views,
                Status = p.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return PaginationHelper.Create(items, pagination, total);
    }
}
