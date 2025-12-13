using AutoMapper; // اضافه شد
using MediatR;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Courses;
using static Pardis.Domain.Dto.Dtos;
using Pardis.Domain.Categories;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Application.Courses.Update
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, OperationResult<CourseResource>>
    {
        private readonly IRepository<Course> _repository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper; // اضافه شد

        public UpdateCourseHandler(IRepository<Course> repository,
                                   IRepository<Category> categoryRepository,
                                   IFileService fileService,
                                   IMapper mapper) // تزریق شد
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<OperationResult<CourseResource>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repository.GetByIdAsync(request.Dto.Id);
            if (course == null) return OperationResult<CourseResource>.NotFound();

            Guid oldCategoryId = course.CategoryId;

            if (request.Dto.Title != null)
            {
                course.Title = request.Dto.Title;
                course.Slug = request.Dto.Title.Replace(" ", "-").ToLower() + "-" + DateTime.Now.Ticks;
            }
            if (request.Dto.Price.HasValue) course.Price = request.Dto.Price.Value;

            if (request.Dto.Description != null) course.Description = request.Dto.Description;

            course.Status = request.Dto.Status;

            var instructorId = request.Dto.InstructorId;

            course.InstructorId = instructorId;
            course.Schedule = request.Dto.Schedule;
            course.StartFrom = request.Dto.StartFrom;
            course.IsCompleted = request.Dto.IsCompleted;
            course.IsStarted = request.Dto.IsStarted;
            course.Location = request.Dto.Location;
            course.Type = request.Dto.Type;

            if (request.Dto.Image != null)
            {
                if (course.Thumbnail != null)
                    _fileService.DeleteFile(Directories.Course, course.Thumbnail);

                string image = await _fileService.SaveFileAndGenerateName(request.Dto.Image, Directories.Course);
                course.Thumbnail = $"/uploads/Corses/Thumbnail/{image}";
            }

            if (request.Dto.CategoryId.HasValue && request.Dto.CategoryId.Value != oldCategoryId)
            {
                var oldCat = await _categoryRepository.GetByIdAsync(oldCategoryId);
                if (oldCat != null) oldCat.CoursesCount--;

                var newCat = await _categoryRepository.GetByIdAsync(request.Dto.CategoryId.Value);
                if (newCat != null) newCat.CoursesCount++;

                course.CategoryId = request.Dto.CategoryId.Value;
                course.Category = newCat; // برای مپینگ صحیح در خروجی
            }

            if (request.Dto.Seo != null)
            {
                // استفاده از مپر برای آپدیت Seo هم ممکن است، اما چون nested است دستی هم مشکلی ندارد
                // یا می‌توانید از _mapper.Map(request.Seo, course.Seo) استفاده کنید
                if (course.Seo == null) course.Seo = new Pardis.Domain.Seo.SeoMetadata();
                _mapper.Map(request.Dto.Seo, course.Seo); // آپدیت سئو با مپر
            }

            if (request.Dto.Sections != null)
            {
                // الف) حذف موارد حذف شده
                // سرفصل‌هایی که در دیتابیس هستند اما در لیست جدید ارسالی نیستند
                var sentIds = request.Dto.Sections.Where(s => s.Id != Guid.Empty).Select(s => s.Id).ToList();
                var sectionsToDelete = course.Sections.Where(s => !sentIds.Contains(s.Id)).ToList();

                foreach (var section in sectionsToDelete)
                {
                    // حذف از کالکشن (EF Core خودش Delete را در دیتابیس اجرا می‌کند)
                    course.Sections.Remove(section);
                }

                // ب) افزودن یا ویرایش موارد
                int orderIndex = 1;
                foreach (var sectionDto in request.Dto.Sections)
                {
                    var existingSection = course.Sections.FirstOrDefault(s => s.Id == sectionDto.Id && s.Id != Guid.Empty);

                    if (existingSection != null)
                    {
                        // --- ویرایش ---
                        existingSection.Title = sectionDto.Title;
                        existingSection.Description = sectionDto.Description;
                        existingSection.Order = orderIndex++;
                    }
                    else
                    {
                        // --- افزودن جدید ---
                        // استفاده از کانستراکتوری که ساختید
                        var newSection = new CourseSection(
                            sectionDto.Title,
                            sectionDto.Description ?? "",
                            orderIndex++,
                            course.Id
                        );
                        course.Sections.Add(newSection);
                    }
                }
            }

            course.UpdatedAt = DateTime.Now;

            await _repository.SaveChangesAsync(cancellationToken);

            // --- تغییر اصلی اینجاست ---
            // به جای آن همه کد دستی، فقط یک خط می‌نویسیم:
            var result = _mapper.Map<CourseResource>(course);
            return OperationResult<CourseResource>.Success(result);
            
        }
    }
}