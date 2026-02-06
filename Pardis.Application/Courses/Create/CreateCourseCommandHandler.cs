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

namespace Pardis.Application.Courses.Create;

public  class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, OperationResult<CourseResource>>
{
    private readonly IRepository<Course> _repository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly ISecureFileService _secureFileService;
    private readonly IMapper _mapper;
    private readonly ISystemLogger _systemLogger;

    public CreateCourseCommandHandler(
        IRepository<Course> repository, 
        IRepository<Category> categoryRepository, 
        ISecureFileService secureFileService, 
        IMapper mapper, 
        ISystemLogger systemLogger)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _secureFileService = secureFileService;
        _mapper = mapper;
        _systemLogger = systemLogger;
    }

    public async Task<OperationResult<CourseResource>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود دسته‌بندی (برای جلوگیری از خطای دیتابیس)
        var categoryCheck = await _categoryRepository.GetByIdAsync(request.Dto.CategoryId);

        if (categoryCheck == null)
        {
            // این خط جلوی ارور Foreign Key را می‌گیرد
            return OperationResult<CourseResource>.Error("دسته‌بندی انتخاب شده معتبر نیست یا حذف شده است.");
        }

        try
        {
            // استفاده از ExecuteInTransactionAsync که با Retry Strategy سازگار است
            var result = await _repository.ExecuteInTransactionAsync(async (ct) =>
            {
                // 1. آپلود تصویر با استفاده از سرویس امن
                string? imagePath = null;
                if (request.Dto.Image != null)
                {
                    var uploadResult = await _secureFileService.SaveFileSecurely(
                        request.Dto.Image, 
                        "courses", 
                        request.CurrentUserId
                    );

                    if (!uploadResult.IsSuccess)
                    {
                        throw new Exception(uploadResult.ErrorMessage ?? "خطا در آپلود تصویر دوره");
                    }

                    imagePath = uploadResult.SecureFileName;
                }

                // 2. تعیین مدرس
                string instructorId = request.CurrentUserId ?? throw new Exception("شناسه کاربر یافت نشد");
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
                    thumbnail: imagePath != null ? $"/uploads/courses/{imagePath}" : null,
                    status: request.Dto.Status,
                    instructorId: instructorId,
                    categoryId: request.Dto.CategoryId,
                    startFrom: request.Dto.StartFrom,
                    location: request.Dto.Location,
                    type: request.Dto.Type,
                    schedule: request.Dto.Schedule ?? string.Empty,
                    isCompleted: request.Dto.IsCompleted,
                    isStarted: request.Dto.IsStarted
                );

                // Add SEO if provided
                if (request.Dto.Seo != null)
                {
                    var seoMetadata = new SeoMetadata(
                        metaTitle: request.Dto.Seo.MetaTitle,
                        metaDescription: request.Dto.Seo.MetaDescription,
                        canonicalUrl: request.Dto.Seo.CanonicalUrl,
                        noIndex: request.Dto.Seo.NoIndex,
                        noFollow: request.Dto.Seo.NoFollow
                    );
                    course.UpdateSeo(seoMetadata);
                }

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
                // SaveChanges در ExecuteInTransactionAsync انجام می‌شود

                // 4. آپدیت شمارنده دسته‌بندی
                var category = await _categoryRepository.GetByIdAsync(request.Dto.CategoryId);
                
                // Log course creation
                await _systemLogger.LogSuccessAsync(
                    $"دوره جدید '{course.Title}' ایجاد شد",
                    "Course",
                    instructorId,
                    "CourseCreated",
                    null,
                    $"CourseId: {course.Id}, Price: {course.Price:N0}, Category: {category?.Title}"
                );
                
                if (category != null)
                {
                    category.CoursesCount++;
                    _categoryRepository.Update(category);
                    // SaveChanges در ExecuteInTransactionAsync انجام می‌شود
                }

                var courseWithRelations = await _repository.Table
                    .AsNoTracking()
                    .Include(c => c.Instructor)
                    .Include(c => c.Category)
                    .Include(c => c.Sections) // ✅ لود کردن سرفصل‌ها
                    .Where(c => c.Id == course.Id)
                    .FirstOrDefaultAsync(ct);

                return _mapper.Map<CourseResource>(courseWithRelations);
            }, cancellationToken);

            return OperationResult<CourseResource>.Success(result);
        }
        catch(Exception e)
        {
            return OperationResult<CourseResource>.Error($"خطا در ایجاد دوره: {e.Message}");
        }
    }
}