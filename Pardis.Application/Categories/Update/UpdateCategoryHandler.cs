using MediatR;
using System;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Seo;
using Pardis.Infrastructure.Repository;
using System.Text.RegularExpressions;

namespace Pardis.Application.Categories.Update;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, OperationResult>
{
    private readonly ICategoryRepository _repository;
    private readonly IRepository<Category> _genericRepository;

    public UpdateCategoryHandler(ICategoryRepository repository, IRepository<Category> genericRepository)
    {
        _repository = repository;
        _genericRepository = genericRepository;
    }

    public async Task<OperationResult> Handle(UpdateCategoryCommand request, CancellationToken token)
    {
        var category = await _repository.GetCategoryWithIdWithSeo(request.Id, token);
        if (category == null)
            return OperationResult.NotFound("????????? ???? ???.");

        if (!string.Equals(category.Title, request.Title, StringComparison.Ordinal))
        {
            var slug = GenerateSlug(request.Title);
            if (await _repository.SlugIsExist(slug))
            {
                slug += "-" + new Random().Next(1000, 9999);
            }
            category.Slug = slug;
        }

        Guid? newParentId = request.ParentId;
        if (newParentId.HasValue && newParentId.Value == Guid.Empty)
        {
            newParentId = null;
        }

        if (newParentId.HasValue)
        {
            var parentExists = await _genericRepository.AnyAsync(c => c.Id == newParentId.Value, token);
            if (!parentExists)
            {
                return OperationResult.Error("????? ???? ????? ???? ?? ???? ?????.");
            }
        }

        category.Title = request.Title;
        category.ParentId = newParentId;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        if (request.Seo != null)
        {
            var seoMetadata = new SeoMetadata(
                metaTitle: request.Seo.MetaTitle,
                metaDescription: request.Seo.MetaDescription,
                canonicalUrl: request.Seo.CanonicalUrl,
                noIndex: request.Seo.NoIndex,
                noFollow: request.Seo.NoFollow
            );
            category.UpdateSeo(seoMetadata);
        }

        await _genericRepository.SaveChangesAsync(token);
        return OperationResult.Success();
    }

    private static string GenerateSlug(string phrase)
    {
        return Regex.Replace(phrase.ToLower().Trim(), @"\s+", "-");
    }
}
