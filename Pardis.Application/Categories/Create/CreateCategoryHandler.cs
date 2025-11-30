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
                return OperationResult.Error("این عنوان قبلا ثبت شده است");

            // 1. ساخت اسلاگ (Slug)
            string slug = GenerateSlug(title);

            // چک کردن یکتا بودن اسلاگ
            if (await _repository.SlugIsExist(slug))
            {
                slug += "-" + new Random().Next(1000, 9999);
            }

            // 2. ایجاد موجودیت
            var category = new Category(
                title,
                slug,
                null, // تصویر فعلا نال
                request.ParentId,
                null, // Parent Object
                new List<Category>(),
                request.CurrentUserId, null, 
                new List<Domain.Courses.Course>()
            )
            {
                IsActive = request.IsActive
            };

            // 3. افزودن سئو
            if (request.Seo != null)
            {
                category.Seo.MetaTitle = request.Seo.MetaTitle;
                category.Seo.MetaDescription = request.Seo.MetaDescription;
                category.Seo.CanonicalUrl = request.Seo.CanonicalUrl;
                category.Seo.NoIndex = request.Seo.NoIndex;
                category.Seo.NoFollow = request.Seo.NoFollow;
            }

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync(token);

            return OperationResult.Success("دسته بندی با موفقیت ثبت شد");
        }

        // متد کمکی برای ساخت Slug از روی متن فارسی/انگلیسی
        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower().Trim();
            // جایگزینی فاصله‌ها با خط تیره
            str = Regex.Replace(str, @"\s+", "-");
            return str;
        }
    }
}
