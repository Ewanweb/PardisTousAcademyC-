using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostsList;

public class GetPostsListHandler : IRequestHandler<GetPostsListQuery, PagedResult<PostListItemDto>>
{
    private readonly IRepository<Post> _postRepository;

    public GetPostsListHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PagedResult<PostListItemDto>> Handle(GetPostsListQuery request, CancellationToken cancellationToken)
    {
        var pagination = PaginationHelper.Normalize(new PaginationRequest { Page = request.Page, PageSize = request.PageSize });

        var query = _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (!request.IncludeDrafts)
            query = query.Where(p => p.Status == PostStatus.Published);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<PostStatus>(request.Status, true, out var status))
                query = query.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
            query = query.Where(p => p.BlogCategory.Slug == request.CategorySlug);

        if (!string.IsNullOrWhiteSpace(request.TagSlug))
            query = query.Where(p => p.PostTags.Any(t => t.Tag.Slug == request.TagSlug));

        if (!string.IsNullOrWhiteSpace(request.Q))
            query = query.Where(p => p.Title.Contains(request.Q) || p.Excerpt.Contains(request.Q) || p.Content.Contains(request.Q));

        if (request.From.HasValue)
            query = query.Where(p => p.PublishedAt >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(p => p.PublishedAt <= request.To.Value);

        query = request.Sort switch
        {
            "oldest" => query.OrderBy(p => p.PublishedAt),
            "mostViewed" => query.OrderByDescending(p => p.Views),
            _ => query.OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
        };

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
