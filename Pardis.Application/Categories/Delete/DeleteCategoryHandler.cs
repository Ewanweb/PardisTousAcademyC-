using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Courses;
using Pardis.Infrastructure.Repository;

namespace Pardis.Application.Categories.Delete
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, OperationResult>
    {
        private readonly ICategoryRepository _repository;
        private readonly ICourseRepository _courseRepository;

        public DeleteCategoryHandler(ICategoryRepository repository, ICourseRepository courseRepository)
        {
            _repository = repository;
            _courseRepository = courseRepository;
        }

        public async Task<OperationResult> Handle(DeleteCategoryCommand request, CancellationToken token)
        {
            try
            {
                return await _repository.ExecuteInTransactionAsync(async (ct) =>
                {
                    var category = await _repository.GetByIdAsync(request.Id);
                    if (category == null) return OperationResult.NotFound("دسته یافت نشد.");

                    // --- سناریوی ۱: درخواست انتقال محتوا وجود دارد ---
                    if (request.MigrateToId.HasValue)
                    {
                        var targetCategory = await _repository.GetByIdAsync(request.MigrateToId.Value);
                        if (targetCategory == null) return OperationResult.Error("دسته‌بندی مقصد یافت نشد.");

                        // چک کردن منطق درختی (جلوگیری از انتقال پدر به فرزند خودش)
                        if (await IsDescendant(request.MigrateToId.Value, request.Id))
                        {
                            return OperationResult.Error("نمی‌توانید محتوا را به یکی از زیرمجموعه‌های همین دسته منتقل کنید.");
                        }

                        // الف) انتقال تمام دوره‌ها به دسته جدید
                        // از ExecuteUpdate برای سرعت بالا استفاده می‌کنیم (EF Core 7+)
                        await _courseRepository.MoveCoursesForDelete(request.Id, request.MigrateToId.Value, ct);

                        // ب) انتقال تمام زیرمجموعه‌ها (فرزندان) به دسته جدید
                        await _repository.MoveCategoryForDelete(request.Id, request.MigrateToId.Value, ct);

                        // نکته: اگر نیاز باشد شمارنده (CoursesCount) دسته مقصد را آپدیت کنید، اینجا انجام دهید.
                    }
                    else
                    {
                        // 1. چک کردن فرزندان (Children)
                        bool hasChildren = await _repository.AnyAsync(
                            // Expression: آیا دسته ای وجود دارد که ParentId آن برابر با Id درخواستی باشد؟
                            c => c.ParentId == request.Id,
                            ct
                        );

                        // 2. چک کردن دوره‌ها (Courses)
                        bool hasCourses = await _courseRepository.AnyAsync(
                            // Expression: آیا دوره‌ای وجود دارد که CategoryId آن برابر با Id درخواستی باشد؟
                            c => c.CategoryId == request.Id,
                            ct
                        );

                        if (hasChildren || hasCourses)
                        {
                            return OperationResult.Error("این دسته خالی نیست! لطفاً شناسه مقصد را ارسال کنید.");
                        }
                    }

                    // حذف نهایی
                    _repository.Remove(category);
                    // SaveChanges در ExecuteInTransactionAsync انجام می‌شود

                    return OperationResult.Success("دسته با موفقیت حذف شد.");
                }, token);
            }
            catch (Exception ex)
            {
                return OperationResult.Error($"خطا در حذف: {ex.Message}");
            }
        }

        // متد کمکی: بررسی می‌کند آیا targetId یکی از نوادگان ancestorId است؟
        private async Task<bool> IsDescendant(Guid targetId, Guid ancestorId)
        {
            var current = await _repository.GetByIdAsync(targetId);
            while (current != null && current.ParentId != null)
            {
                if (current.ParentId == ancestorId)
                {
                    return true; // بله، به جد خود رسیدیم
                }
                // حرکت به سمت بالا (پدر)
                current = await _repository.GetByIdAsync(current.ParentId);
            }
            return false;
        }
    }
}
