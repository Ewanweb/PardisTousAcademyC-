using MediatR;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Domain.Courses;

namespace Pardis.Application.Courses;

public partial class SoftDeleteCommandHandler
{
    // تعریف Handler
    public class ForceDeleteCourseHandler : IRequestHandler<ForceDeleteCourseCommand, OperationResult>
    {
        private readonly ICourseRepository _repository;
        private readonly IFileService _fileService; // سرویس مدیریت فایل

        public ForceDeleteCourseHandler(ICourseRepository repository, IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<OperationResult> Handle(ForceDeleteCourseCommand request, CancellationToken token)
        {
            using var transaction = _repository.BeginTransaction();

            try
            {
                // پیدا کردن دوره (حتی اگر سافت دیلیت شده باشد)
                var course = await _repository.GetDeletedCourseById(request.Id, token);

                if (course == null) return OperationResult.NotFound("دوره یافت نشد.");


                if (!string.IsNullOrEmpty(course.Thumbnail))
                {
                    _fileService.DeleteFile(Directories.Course, course.Thumbnail);
                }


                // 3. حذف نهایی از دیتابیس
                // نکته: چون Seo به صورت Owned Type تعریف شده، با حذف دوره، سئو هم خودکار حذف می‌شود.
                _repository.Remove(course);

                await _repository.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                return OperationResult.Success("دوره به طور کامل حذف شد.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                // نکته: اگر فایل حذف شده باشد، قابل بازگشت نیست (رول‌بک فقط دیتابیس را برمی‌گرداند)
                return OperationResult.Error($"خطا در حذف دائم: {ex.Message}");
            }
        }
    }
}