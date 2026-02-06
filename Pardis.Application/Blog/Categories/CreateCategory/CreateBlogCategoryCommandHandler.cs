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

namespace Pardis.Application.Blog.Categories.CreateCategory;

public class CreateBlogCategoryCommandHandler : IRequestHandler<CreateBlogCategoryCommand, OperationResult<BlogCategoryDto>>
{
    private readonly IRepository<BlogCategory> _categoryRepository;

    public CreateBlogCategoryCommandHandler(IRepository<BlogCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<OperationResult<BlogCategoryDto>> Handle(CreateBlogCategoryCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateCategoryRequestValidator();
        var validation = await validator.ValidateAsync(request.Dto, cancellationToken);
        if (!validation.IsValid)
            return OperationResult<BlogCategoryDto>.Error(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        var slug = SlugHelper.Normalize(string.IsNullOrWhiteSpace(request.Dto.Slug) ? request.Dto.Title : request.Dto.Slug);
        if (string.IsNullOrWhiteSpace(slug))
            return OperationResult<BlogCategoryDto>.Error("اسلاگ معتبر نیست");

        var exists = await _categoryRepository.Table.AnyAsync(c => c.Slug == slug, cancellationToken);
        if (exists)
            return OperationResult<BlogCategoryDto>.Error("اسلاگ تکراری است");

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
            openGraphType: seo.OgType ?? "website",
            twitterTitle: seo.TwitterTitle,
            twitterDescription: seo.TwitterDescription,
            twitterImage: seo.TwitterImage,
            twitterCardType: seo.TwitterCard ?? "summary_large_image"
        );

        var category = new BlogCategory
        {
            Title = request.Dto.Title.Trim(),
            Slug = slug,
            Description = request.Dto.Description,
            Priority = request.Dto.Priority,
            ThumbnailUrl = request.Dto.CoverImageUrl ?? string.Empty,
            CreatedBy = request.CurrentUserId ?? "System",
            UserId = request.CurrentUserId ?? string.Empty,
            SeoMetadata = seoMetadata,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        var dto = new BlogCategoryDto
        {
            Id = category.Id,
            Title = category.Title,
            Slug = category.Slug,
            Description = category.Description,
            Priority = category.Priority,
            Seo = new SeoDto
            {
                MetaTitle = category.SeoMetadata.MetaTitle,
                MetaDescription = category.SeoMetadata.MetaDescription,
                CanonicalUrl = category.SeoMetadata.CanonicalUrl,
                NoIndex = category.SeoMetadata.NoIndex,
                NoFollow = category.SeoMetadata.NoFollow
            }
        };

        return OperationResult<BlogCategoryDto>.Success(dto);
    }
}
