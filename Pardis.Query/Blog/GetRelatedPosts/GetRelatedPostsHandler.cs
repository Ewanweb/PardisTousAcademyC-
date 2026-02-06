using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetRelatedPosts;

public class GetRelatedPostsHandler : IRequestHandler<GetRelatedPostsQuery, List<PostListItemDto>>
{
    private readonly IRepository<Post> _postRepository;

    public GetRelatedPostsHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostListItemDto>> Handle(GetRelatedPostsQuery request, CancellationToken cancellationToken)
    {
        var current = await _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published && p.Slug == request.Slug)
            .Select(p => new
            {
                p.Id,
                p.BlogCategoryId,
                TagIds = p.PostTags.Select(t => t.TagId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (current == null)
            return new List<PostListItemDto>();

        var query = _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published && p.Id != current.Id)
            .Where(p => p.BlogCategoryId == current.BlogCategoryId || p.PostTags.Any(t => current.TagIds.Contains(t.TagId)))
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Take(Math.Max(1, request.Take));

        return await query
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
    }
}
