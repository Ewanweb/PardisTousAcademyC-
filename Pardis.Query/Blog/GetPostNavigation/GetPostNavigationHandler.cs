using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostNavigation;

public class GetPostNavigationHandler : IRequestHandler<GetPostNavigationQuery, PostNavDto>
{
    private readonly IRepository<Post> _postRepository;

    public GetPostNavigationHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostNavDto> Handle(GetPostNavigationQuery request, CancellationToken cancellationToken)
    {
        var current = await _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published && p.Slug == request.Slug)
            .Select(p => new { p.PublishedAt, p.CreatedAt })
            .FirstOrDefaultAsync(cancellationToken);

        if (current == null)
            return new PostNavDto();

        var anchor = current.PublishedAt ?? current.CreatedAt;

        var next = await _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published && (p.PublishedAt ?? p.CreatedAt) > anchor)
            .OrderBy(p => p.PublishedAt ?? p.CreatedAt)
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
            .FirstOrDefaultAsync(cancellationToken);

        var previous = await _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published && (p.PublishedAt ?? p.CreatedAt) < anchor)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
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
            .FirstOrDefaultAsync(cancellationToken);

        return new PostNavDto
        {
            Next = next,
            Previous = previous
        };
    }
}
