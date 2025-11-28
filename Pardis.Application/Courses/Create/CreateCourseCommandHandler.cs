using MediatR;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;

namespace Pardis.Application.Courses.Create;

public  class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, OperationResult>
{
    private readonly IRepository<Course> _repository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IFileService _fileService;

    public CreateCourseCommandHandler(IRepository<Course> repository, IRepository<Category> categoryRepository, IFileService fileService)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _fileService = fileService;
    }

    public async Task<OperationResult> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = _repository.BeginTransaction();
        try
        {
            // 1. آپلود تصویر
            string? imagePath = await _fileService.SaveFileAndGenerateName(request.Dto.Image, Directories.Course);

            // 2. تعیین مدرس
            string? instructorId = request.CurrentUser.Id;
            if (request.IsAdmin && !string.IsNullOrEmpty(request.Dto.InstructorId))
            {
                instructorId = request.Dto.InstructorId;
            }

            // 3. ساخت اسلاگ
            string slug = request.Dto.Title.Replace(" ", "-").ToLower() + "-" + DateTime.Now.Ticks;

            var course = new Course(
                title: request.Dto.Title,
                slug: slug,
                description: request.Dto.Description,
                price: request.Dto.Price,
                thumbnail: imagePath,
                status: request.Dto.Status,
                instructorId: instructorId,
                categoryId: request.Dto.CategoryId,
                seo: new SeoMetadata
                {
                    MetaTitle = request.Dto.Seo?.MetaTitle,
                    MetaDescription = request.Dto.Seo?.MetaDescription,
                    CanonicalUrl = request.Dto.Seo?.CanonicalUrl,
                    NoIndex = request.Dto.Seo?.NoIndex ?? false,
                    NoFollow = request.Dto.Seo?.NoFollow ?? false
                }
            );

            await _repository.AddAsync(course);
            await _repository.SaveChangesAsync(cancellationToken);

            // 4. آپدیت شمارنده دسته‌بندی
            var category = await _categoryRepository.GetByIdAsync(request.Dto.CategoryId);
            if (category != null)
            {
                category.CoursesCount++;
                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return OperationResult.Success();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            return OperationResult.Error("خطا غیر منتظره");
        }
    }
}