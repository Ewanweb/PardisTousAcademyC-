using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;

namespace Pardis.Application.Courses;

public class SoftDeleteCommandHandler : IRequestHandler<SoftDeleteCommand, OperationResult>
{
    private readonly IRepository<Course> _repository;
    private readonly IRepository<Category> _categoryRepository;

    public SoftDeleteCommandHandler(IRepository<Course> repository, IRepository<Category> categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<OperationResult> Handle(SoftDeleteCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id);
        if (course == null) return OperationResult.NotFound("دوره ای یافت نشد");

        course.IsDeleted = true;
        course.DeletedAt = DateTime.UtcNow;

        // کاهش شمارنده
        var category = await _categoryRepository.GetByIdAsync(course.CategoryId);
        if (category != null)
        {
            category.CoursesCount = Math.Max(0, category.CoursesCount - 1);
        }

        await _repository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }
}