using Pardis.Domain.Seo;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Categories;
using Pardis.Infrastructure.Repository;
using System.Text.RegularExpressions;

namespace Pardis.Application.Categories.Create
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, OperationResult>
    {
        private readonly ICategoryRepository _repository;

        public CreateCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> Handle(CreateCategoryCommand request, CancellationToken token)
        {
            string title = request.Title;

            if (await _repository.IsExist(title))
                return OperationResult.Error("این دسته بندی از قبل ایجاد شده");

            string slug = GenerateSlug(title);

            if (await _repository.SlugIsExist(slug))
            {
                slug += "-" + new Random().Next(1000, 9999);
            }

            var category = new Category(
                title,
                slug,
                null, 
                request.ParentId,
                request.CurrentUserId,
                request.Description
            )
            {
                IsActive = request.IsActive
            };

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

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync(token);

            return OperationResult.Success("دسته بندی با موفقیت ایجاد شد");
        }

        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower().Trim();
            str = Regex.Replace(str, @"\s+", "-");
            return str;
        }
    }
}

