using MediatR;
using Pardis.Application._Shared;
using Pardis.Infrastructure.Repository;
using System.Text.RegularExpressions;

namespace Pardis.Application.Categories.Update
{
    using global::Pardis.Domain;
    using global::Pardis.Domain.Categories;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Text.RegularExpressions;
    using static global::Pardis.Domain.Dto.Dtos;

    namespace Pardis.Application.Categories.Commands
    {

        public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, OperationResult>
        {
            private readonly ICategoryRepository _repository; // تغییر: تزریق واسط ریپازیتوری
            private readonly IRepository<Category> _genericRepository; // برای استفاده از SaveChangesAsync و AnyAsync جنریک

            public UpdateCategoryHandler(ICategoryRepository repository, IRepository<Category> genericRepository)
            {
                _repository = repository;
                _genericRepository = genericRepository;
            }

            public async Task<OperationResult> Handle(UpdateCategoryCommand request, CancellationToken token)
            {
                // تغییر: استفاده از متد تخصصی ریپازیتوری برای دریافت دسته با سئو
                var category = await _repository.GetCategoryWithIdWithSeo(request.Id, token);
                if (category == null) return OperationResult.NotFound("دسته‌بندی یافت نشد.");

                // لاجیک آپدیت اسلاگ: اگر نام تغییر کرده بود
                if (category.Title != request.Title)
                {
                    string slug = GenerateSlug(request.Title);

                    // تغییر: استفاده از متد تخصصی ریپازیتوری برای چک کردن وجود Slug (بدون خودش)
                    bool exists = await _repository.SlugIsExist(slug);

                    if (exists)
                    {
                        slug += "-" + new Random().Next(1000, 9999);
                    }
                    category.Slug = slug;
                }

                // --- لاجیک بررسی کلید خارجی ---
                Guid? newParentId = request.ParentId;

                // اگر ID فرستاده شده GUID.Empty بود (0000...)، آن را به null تبدیل کن.
                if (newParentId.HasValue && newParentId.Value == Guid.Empty)
                {
                    newParentId = null;
                }

                // چک می‌کنیم که اگر ParentId فرستاده شده نال نیست، آیا در دیتابیس وجود دارد؟
                if (newParentId.HasValue)
                {
                    // استفاده از ریپازیتوری جنریک برای چک کردن وجود (به جای دسترسی به Context)
                    var parentExists = await _genericRepository.AnyAsync(c => c.Id == newParentId.Value, token);
                    if (!parentExists)
                    {
                        return OperationResult.Error("شناسه والد (ParentId) نامعتبر است و در دیتابیس یافت نشد.");
                    }
                }
                // ----------------------------------------------


                // آپدیت فیلدها
                category.Title = request.Title;
                category.ParentId = newParentId; // استفاده از ParentId اصلاح شده
                category.IsActive = request.IsActive;
                category.UpdatedAt = DateTime.UtcNow;

                // آپدیت سئو
                if (request.Seo != null)
                {
                    category.Seo.MetaTitle = request.Seo.MetaTitle;
                    category.Seo.MetaDescription = request.Seo.MetaDescription;
                    category.Seo.CanonicalUrl = request.Seo.CanonicalUrl;
                    category.Seo.NoIndex = request.Seo.NoIndex;
                    category.Seo.NoFollow = request.Seo.NoFollow;
                }

                // تغییر: استفاده از SaveChangesAsync جنریک
                await _genericRepository.SaveChangesAsync(token);
                return OperationResult.Success();
            }

            private string GenerateSlug(string phrase)
            {
                return Regex.Replace(phrase.ToLower().Trim(), @"\s+", "-");
            }
        }
    }
}
