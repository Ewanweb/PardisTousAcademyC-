using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Seo;

namespace Pardis.Query.Blog.GetPostBySlug;

public class GetPostBySlugHandler : IRequestHandler<GetPostBySlugQuery, PostSlugResolveDto>
{
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<PostSlugHistory> _slugHistoryRepository;

    public GetPostBySlugHandler(IRepository<Post> postRepository, IRepository<PostSlugHistory> slugHistoryRepository)
    {
        _postRepository = postRepository;
        _slugHistoryRepository = slugHistoryRepository;
    }

    public async Task<PostSlugResolveDto> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var query = _postRepository.Table
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (!request.IncludeDrafts)
            query = query.Where(p => p.Status == PostStatus.Published);

        var post = await query
            .Where(p => p.Slug == request.Slug)
            .Select(p => new PostDetailDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Content = p.Content,
                Excerpt = p.Excerpt,
                CoverImageUrl = p.CoverImageUrl,
                Author = p.Author,
                PublishedAt = p.PublishedAt,
                ReadingTimeMinutes = p.ReadingTimeMinutes,
                ViewCount = p.Views,
                Status = p.Status.ToString(),
                Category = new BlogCategoryDto
                {
                    Id = p.BlogCategory.Id,
                    Title = p.BlogCategory.Title,
                    Slug = p.BlogCategory.Slug,
                    Description = p.BlogCategory.Description,
                    Priority = p.BlogCategory.Priority,
                    Seo = new SeoDto
                    {
                        MetaTitle = p.BlogCategory.SeoMetadata.MetaTitle,
                        MetaDescription = p.BlogCategory.SeoMetadata.MetaDescription,
                        CanonicalUrl = p.BlogCategory.SeoMetadata.CanonicalUrl,
                        NoIndex = p.BlogCategory.SeoMetadata.NoIndex,
                        NoFollow = p.BlogCategory.SeoMetadata.NoFollow
                    }
                },
                Tags = p.PostTags.Select(pt => new TagDto
                {
                    Id = pt.Tag.Id,
                    Title = pt.Tag.Title,
                    Slug = pt.Tag.Slug
                }).ToList(),
                Seo = new SeoDto
                {
                    MetaTitle = p.SeoMetadata.MetaTitle,
                    MetaDescription = p.SeoMetadata.MetaDescription,
                    CanonicalUrl = p.SeoMetadata.CanonicalUrl,
                    NoIndex = p.SeoMetadata.NoIndex,
                    NoFollow = p.SeoMetadata.NoFollow,
                    OgTitle = p.SeoMetadata.OpenGraphTitle,
                    OgDescription = p.SeoMetadata.OpenGraphDescription,
                    OgImage = p.SeoMetadata.OpenGraphImage,
                    OgType = p.SeoMetadata.OpenGraphType,
                    TwitterTitle = p.SeoMetadata.TwitterTitle,
                    TwitterDescription = p.SeoMetadata.TwitterDescription,
                    TwitterImage = p.SeoMetadata.TwitterImage,
                    TwitterCard = p.SeoMetadata.TwitterCardType
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (post != null)
        {
            return new PostSlugResolveDto
            {
                Post = post,
                IsRedirect = false
            };
        }

        var history = await _slugHistoryRepository.Table
            .AsNoTracking()
            .Where(h => h.OldSlug == request.Slug)
            .OrderByDescending(h => h.ChangedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (history != null)
        {
            return new PostSlugResolveDto
            {
                Post = null,
                IsRedirect = true,
                RedirectSlug = history.NewSlug
            };
        }

        return new PostSlugResolveDto
        {
            Post = null,
            IsRedirect = false
        };
    }
}
