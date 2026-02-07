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

namespace Pardis.Application.Blog.Categories.UpdateCategory;

public class UpdateBlogCategoryCommandHandler : IRequestHandler<UpdateBlogCategoryCommand, OperationResult<BlogCategoryDto>>
{
    private readonly IRepository<BlogCategory> _categoryRepository;

    public UpdateBlogCategoryCommandHandler(IRepository<BlogCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<OperationResult<BlogCategoryDto>> Handle(UpdateBlogCategoryCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateCategoryRequestValidator();
        var validation = await validator.ValidateAsync(request.Dto, cancellationToken);
        if (!validation.IsValid)
            return OperationResult<BlogCategoryDto>.Error(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        var category = await _categoryRepository.Table.FirstOrDefaultAsync(c => c.Id == request.CategoryId && !c.IsDeleted, cancellationToken);
        if (category == null)
            return OperationResult<BlogCategoryDto>.NotFound("دسته‌بندی یافت نشد");

        var slug = SlugHelper.Normalize(string.IsNullOrWhiteSpace(request.Dto.Slug) ? request.Dto.Title : request.Dto.Slug);
        if (string.IsNullOrWhiteSpace(slug))
            return OperationResult<BlogCategoryDto>.Error("اسلاگ معتبر نیست");

        if (!string.Equals(category.Slug, slug, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _categoryRepository.Table.AnyAsync(c => c.Slug == slug && c.Id != category.Id, cancellationToken);
            if (exists)
                return OperationResult<BlogCategoryDto>.Error("اسلاگ تکراری است");

            category.Slug = slug;
        }

        category.Title = request.Dto.Title.Trim();
        category.Description = request.Dto.Description;
        category.Priority = request.Dto.Priority;
        category.ThumbnailUrl = request.Dto.CoverImageUrl ?? category.ThumbnailUrl;
        category.UpdatedAt = DateTime.UtcNow;

        var seo = request.Dto.Seo ?? new SeoDto();
        category.SeoMetadata = new SeoMetadata(
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

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        var dto = new BlogCategoryDto
        {
            Id = category.Id,
            Title = category.Title,
            Slug = category.Slug,
            Description = category.Description,
            Priority = category.Priority,
            CoverImageUrl = category.ThumbnailUrl,
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
