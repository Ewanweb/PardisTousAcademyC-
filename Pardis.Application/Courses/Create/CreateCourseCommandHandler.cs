using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Seo;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Courses.Create;

public  class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, OperationResult<CourseResource>>
{
    private readonly IRepository<Course> _repository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public CreateCourseCommandHandler(IRepository<Course> repository, IRepository<Category> categoryRepository, IFileService fileService, IMapper mapper)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<OperationResult<CourseResource>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود دسته‌بندی (برای جلوگیری از خطای دیتابیس)
        var categoryCheck = await _categoryRepository.GetByIdAsync(request.Dto.CategoryId);

        if (categoryCheck == null)
        {
            // این خط جلوی ارور Foreign Key را می‌گیرد
            throw new Exception("دسته‌بندی انتخاب شده معتبر نیست یا حذف شده است.");
        }

        await using var transaction = _repository.BeginTransaction();
        try
        {
            // 1. آپلود تصویر
            string? imagePath = await _fileService.SaveFileAndGenerateName(request.Dto.Image, Directories.Course);

            // 2. تعیین مدرس
            string? instructorId = request.CurrentUserId;
            if (request.IsAdmin && !string.IsNullOrEmpty(request.Dto.InstructorId))
            {
                instructorId = request.Dto.InstructorId;
            }

            // 3. ساخت اسلاگ
            string slug = request.Dto.Title.Replace(" ", "-").ToLower();


            var course = new Course(
                title: request.Dto.Title,
                slug: slug,
                description: request.Dto.Description,
                price: request.Dto.Price,
                thumbnail: $"/{Directories.Course}/{imagePath}",
                status: request.Dto.Status,
                instructorId: instructorId,
                categoryId: request.Dto.CategoryId,
                startFrom: request.Dto.StartFrom,
                schedule: request.Dto.Schedule,
                isCompleted: request.Dto.IsCompleted,
                isStarted: request.Dto.IsStarted,
                seo: new SeoMetadata
                {
                    MetaTitle = request.Dto.Seo?.MetaTitle,
                    MetaDescription = request.Dto.Seo?.MetaDescription,
                    CanonicalUrl = request.Dto.Seo?.CanonicalUrl,
                    NoIndex = request.Dto.Seo?.NoIndex ?? false,
                    NoFollow = request.Dto.Seo?.NoFollow ?? false
                }
            );

            if (request.Dto.Sections != null && request.Dto.Sections.Any())
            {
                int orderIndex = 1;
                foreach (var sectionDto in request.Dto.Sections)
                {
                    // ایجاد موجودیت سرفصل
                    var section = new CourseSection(sectionDto.Title, sectionDto.Description, orderIndex++, course.Id);

                    // افزودن به کالکشن دوره
                    course.Sections.Add(section);
                }
            }

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

            var courseWithRelations = await _repository.Table
                .AsNoTracking()
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Sections) // ✅ لود کردن سرفصل‌ها
                .Where(c => c.Id == course.Id)
                .FirstOrDefaultAsync(cancellationToken);


            var result = _mapper.Map<CourseResource>(courseWithRelations);
            await transaction.CommitAsync(cancellationToken);
            return OperationResult<CourseResource>.Success(result);
        }
        catch(Exception e)
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);

            return OperationResult<CourseResource>.Error($"{e}");
        }
    }
}