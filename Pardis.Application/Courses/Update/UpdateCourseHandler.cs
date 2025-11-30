using AutoMapper; // اضافه شد
using MediatR;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Courses;
using static Pardis.Domain.Dto.Dtos;
using Pardis.Domain.Categories;
using Pardis.Application._Shared;

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
            var course = await _repository.GetByIdAsync(request.Id);
            if (course == null) return OperationResult<CourseResource>.NotFound();

            Guid oldCategoryId = course.CategoryId;

            if (request.Title != null)
            {
                course.Title = request.Title;
                course.Slug = request.Title.Replace(" ", "-").ToLower() + "-" + DateTime.Now.Ticks;
            }
            if (request.Price.HasValue) course.Price = request.Price.Value;
            if (request.Description != null) course.Description = request.Description;
            course.Status = request.Status;

            if (request.IsAdmin && !string.IsNullOrEmpty(request.InstructorId))
            {
                course.InstructorId = request.InstructorId;
            }

            if (request.Image != null)
            {
                if (course.Thumbnail != null)
                    _fileService.DeleteFile(Directories.Course, course.Thumbnail);

                string image = await _fileService.SaveFileAndGenerateName(request.Image, Directories.Course);
                course.Thumbnail = image;
            }

            if (request.CategoryId.HasValue && request.CategoryId.Value != oldCategoryId)
            {
                var oldCat = await _categoryRepository.GetByIdAsync(oldCategoryId);
                if (oldCat != null) oldCat.CoursesCount--;

                var newCat = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (newCat != null) newCat.CoursesCount++;

                course.CategoryId = request.CategoryId.Value;
                course.Category = newCat; // برای مپینگ صحیح در خروجی
            }

            if (request.Seo != null)
            {
                // استفاده از مپر برای آپدیت Seo هم ممکن است، اما چون nested است دستی هم مشکلی ندارد
                // یا می‌توانید از _mapper.Map(request.Seo, course.Seo) استفاده کنید
                if (course.Seo == null) course.Seo = new Pardis.Domain.Seo.SeoMetadata();
                _mapper.Map(request.Seo, course.Seo); // آپدیت سئو با مپر
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