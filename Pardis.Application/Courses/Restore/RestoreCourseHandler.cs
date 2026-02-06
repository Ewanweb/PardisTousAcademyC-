using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using System;

namespace Pardis.Application.Courses;

public partial class SoftDeleteCommandHandler
{
    public class RestoreCourseHandler : IRequestHandler<RestoreCourseCommand, OperationResult>
    {
        private readonly ICourseRepository _repository;
        private readonly IRepository<Category> _categoryRepository;

        public RestoreCourseHandler(ICourseRepository repository, IRepository<Category> categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        public async Task<OperationResult> Handle(RestoreCourseCommand request, CancellationToken token)
        {
            try
            {
                return await _repository.ExecuteInTransactionAsync(async (ct) =>
                {
                    var course = await _repository.GetDeletedCourseById(request.Id, ct);

                    if (course == null) return OperationResult.NotFound("دوره یافت نشد.");

                    if (!course.IsDeleted) return OperationResult.Error("این دوره حذف نشده است.");

                    course.IsDeleted = false;
                    course.DeletedAt = null;

                    if (course.CategoryId != Guid.Empty)
                    {
                        var category = await _categoryRepository.GetByIdAsync(course.CategoryId);
                        if (category != null)
                        {
                            category.CoursesCount++;
                        }
                    }

                    // SaveChanges در ExecuteInTransactionAsync انجام می‌شود
                    return OperationResult.Success("دوره با موفقیت بازیابی شد.");
                }, token);
            }
            catch (Exception ex)
            {
                return OperationResult.Error($"خطا در بازیابی: {ex.Message}");
            }
        }
    }
}