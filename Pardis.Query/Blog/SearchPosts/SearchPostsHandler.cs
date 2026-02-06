using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.SearchPosts;

public class SearchPostsHandler : IRequestHandler<SearchPostsQuery, PagedResult<PostListItemDto>>
{
    private readonly IRepository<Post> _postRepository;

    public SearchPostsHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PagedResult<PostListItemDto>> Handle(SearchPostsQuery request, CancellationToken cancellationToken)
    {
        var pagination = PaginationHelper.Normalize(new PaginationRequest { Page = request.Page, PageSize = request.PageSize });
        var q = request.Q?.Trim() ?? string.Empty;

        var query = _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published);

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(p => p.Title.Contains(q) || p.Excerpt.Contains(q) || p.Content.Contains(q));
        }

        query = query.OrderByDescending(p => p.PublishedAt ?? p.CreatedAt);

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
                Status = p.Status.ToString(),
                HighlightedTitle = Highlight(p.Title, q),
                HighlightedExcerpt = Highlight(p.Excerpt, q)
            })
            .ToListAsync(cancellationToken);

        return PaginationHelper.Create(items, pagination, total);
    }

    private static string? Highlight(string input, string query)
    {
        if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(input))
            return null;

        var index = input.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return null;

        return input.Substring(0, index) + "<mark>" +
               input.Substring(index, query.Length) +
               "</mark>" + input[(index + query.Length)..];
    }
}
