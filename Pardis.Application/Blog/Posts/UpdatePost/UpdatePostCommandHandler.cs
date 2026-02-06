using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Application.Blog.Utils;
using Pardis.Application.Blog.Validation;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Seo;

namespace Pardis.Application.Blog.Posts.UpdatePost;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, OperationResult<PostDetailDto>>
{
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<BlogCategory> _categoryRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<PostSlugHistory> _slugHistoryRepository;

    public UpdatePostCommandHandler(
        IRepository<Post> postRepository,
        IRepository<BlogCategory> categoryRepository,
        IRepository<Tag> tagRepository,
        IRepository<PostSlugHistory> slugHistoryRepository)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _slugHistoryRepository = slugHistoryRepository;
    }

    public async Task<OperationResult<PostDetailDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdatePostRequestValidator();
        var validation = await validator.ValidateAsync(request.Dto, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationResult<PostDetailDto>.Error(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var post = await _postRepository.Table
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);

        if (post == null)
            return OperationResult<PostDetailDto>.NotFound("مطلب یافت نشد");

        var category = await _categoryRepository.GetByIdAsync(request.Dto.BlogCategoryId);
        if (category == null || category.IsDeleted)
            return OperationResult<PostDetailDto>.Error("دسته‌بندی معتبر نیست");

        var normalizedSlug = SlugHelper.Normalize(string.IsNullOrWhiteSpace(request.Dto.Slug) ? request.Dto.Title : request.Dto.Slug);
        if (string.IsNullOrWhiteSpace(normalizedSlug))
            return OperationResult<PostDetailDto>.Error("اسلاگ معتبر نیست");

        if (!string.Equals(post.Slug, normalizedSlug, StringComparison.OrdinalIgnoreCase))
        {
            var slugExists = await _postRepository.Table.AnyAsync(p => p.Slug == normalizedSlug && p.Id != post.Id, cancellationToken);
            if (slugExists)
                return OperationResult<PostDetailDto>.Error("اسلاگ تکراری است");

            await _slugHistoryRepository.AddAsync(new PostSlugHistory
            {
                PostId = post.Id,
                OldSlug = post.Slug,
                NewSlug = normalizedSlug,
                ChangedAt = DateTime.UtcNow
            });

            post.Slug = normalizedSlug;
        }

        post.Title = request.Dto.Title.Trim();
        post.Content = request.Dto.Content.Trim();
        post.Excerpt = request.Dto.Excerpt.Trim();
        post.Description = post.Content;
        post.SummaryDescription = post.Excerpt;
        post.CoverImageUrl = request.Dto.CoverImageUrl;
        post.ThumbnailUrl = request.Dto.CoverImageUrl ?? post.ThumbnailUrl;
        post.BlogCategoryId = category.Id;
        post.Status = ParseStatus(request.Dto.Status);

        if (post.Status == PostStatus.Published)
            post.PublishedAt ??= request.Dto.PublishedAt ?? DateTime.UtcNow;
        else
            post.PublishedAt = request.Dto.PublishedAt;

        var seo = request.Dto.Seo ?? new SeoDto();
        post.SeoMetadata = new SeoMetadata(
            metaTitle: seo.MetaTitle,
            metaDescription: seo.MetaDescription,
            canonicalUrl: seo.CanonicalUrl,
            noIndex: seo.NoIndex,
            noFollow: seo.NoFollow,
            openGraphTitle: seo.OgTitle,
            openGraphDescription: seo.OgDescription,
            openGraphImage: seo.OgImage,
            openGraphType: seo.OgType ?? "article",
            twitterTitle: seo.TwitterTitle,
            twitterDescription: seo.TwitterDescription,
            twitterImage: seo.TwitterImage,
            twitterCardType: seo.TwitterCard ?? "summary_large_image"
        );

        post.UpdateReadingTime();
        post.UpdatedAt = DateTime.UtcNow;

        await UpdateTagsAsync(post, request.Dto.Tags, cancellationToken);

        _postRepository.Update(post);
        await _postRepository.SaveChangesAsync(cancellationToken);

        var dto = await _postRepository.Table
            .AsNoTracking()
            .Where(p => p.Id == post.Id)
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
            .FirstAsync(cancellationToken);

        return OperationResult<PostDetailDto>.Success(dto);
    }

    private static PostStatus ParseStatus(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "published" => PostStatus.Published,
            "archived" => PostStatus.Archived,
            _ => PostStatus.Draft
        };
    }

    private async Task UpdateTagsAsync(Post post, List<string> tags, CancellationToken cancellationToken)
    {
        post.PostTags.Clear();

        if (tags == null || tags.Count == 0)
            return;

        var normalized = tags.Select(SlugHelper.Normalize).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        var existing = await _tagRepository.Table.Where(t => normalized.Contains(t.Slug)).ToListAsync(cancellationToken);

        var missing = normalized.Except(existing.Select(t => t.Slug)).ToList();
        foreach (var slug in missing)
        {
            var title = slug.Replace('-', ' ');
            var tag = new Tag { Title = title, Slug = slug };
            await _tagRepository.AddAsync(tag);
            existing.Add(tag);
        }

        foreach (var tag in existing)
        {
            post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tag.Id });
        }
    }
}
