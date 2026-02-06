using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Application.Blog.Utils;
using Pardis.Application.Blog.Validation;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;

namespace Pardis.Application.Blog.Posts.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, OperationResult<PostDetailDto>>
{
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<BlogCategory> _categoryRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly UserManager<User> _userManager;

    public CreatePostCommandHandler(
        IRepository<Post> postRepository,
        IRepository<BlogCategory> categoryRepository,
        IRepository<Tag> tagRepository,
        UserManager<User> userManager)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _userManager = userManager;
    }

    public async Task<OperationResult<PostDetailDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreatePostRequestValidator();
        var validation = await validator.ValidateAsync(request.Dto, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationResult<PostDetailDto>.Error(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (string.IsNullOrEmpty(request.CurrentUserId))
            return OperationResult<PostDetailDto>.Error("شناسه نویسنده یافت نشد");

        var author = await _userManager.FindByIdAsync(request.CurrentUserId!);
        if (author == null)
            return OperationResult<PostDetailDto>.Error("نویسنده یافت نشد");

        var category = await _categoryRepository.GetByIdAsync(request.Dto.BlogCategoryId);
        if (category == null || category.IsDeleted)
            return OperationResult<PostDetailDto>.Error("دسته‌بندی معتبر نیست");

        var normalizedSlug = SlugHelper.Normalize(string.IsNullOrWhiteSpace(request.Dto.Slug) ? request.Dto.Title : request.Dto.Slug);
        if (string.IsNullOrWhiteSpace(normalizedSlug))
            return OperationResult<PostDetailDto>.Error("اسلاگ معتبر نیست");

        var slugExists = await _postRepository.Table.AnyAsync(p => p.Slug == normalizedSlug, cancellationToken);
        if (slugExists)
            return OperationResult<PostDetailDto>.Error("اسلاگ تکراری است");

        var seo = request.Dto.Seo ?? new SeoDto();
        var seoMetadata = new SeoMetadata(
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

        var post = new Post
        {
            UserId = request.CurrentUserId!,
            Author = author.FullName ?? author.UserName ?? "Author",
            BlogCategoryId = category.Id,
            Title = request.Dto.Title.Trim(),
            Slug = normalizedSlug,
            Content = request.Dto.Content.Trim(),
            Excerpt = request.Dto.Excerpt.Trim(),
            CoverImageUrl = request.Dto.CoverImageUrl,
            Description = request.Dto.Content.Trim(),
            SummaryDescription = request.Dto.Excerpt.Trim(),
            ThumbnailUrl = request.Dto.CoverImageUrl ?? string.Empty,
            Status = ParseStatus(request.Dto.Status),
            PublishedAt = request.Dto.PublishedAt,
            SeoMetadata = seoMetadata,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (post.Status == PostStatus.Published && post.PublishedAt == null)
            post.PublishedAt = DateTime.UtcNow;

        post.UpdateReadingTime();

        await _postRepository.AddAsync(post);

        var tags = await EnsureTagsAsync(request.Dto.Tags, cancellationToken);
        foreach (var tag in tags)
        {
            post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tag.Id });
        }

        await _postRepository.SaveChangesAsync(cancellationToken);

        var dto = await ProjectPostDetailAsync(post.Id, cancellationToken);
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

    private async Task<List<Tag>> EnsureTagsAsync(List<string> tags, CancellationToken cancellationToken)
    {
        if (tags == null || tags.Count == 0)
            return new List<Tag>();

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

        return existing;
    }

    private async Task<PostDetailDto> ProjectPostDetailAsync(Guid postId, CancellationToken cancellationToken)
    {
        return await _postRepository.Table
            .AsNoTracking()
            .Where(p => p.Id == postId)
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
    }
}
