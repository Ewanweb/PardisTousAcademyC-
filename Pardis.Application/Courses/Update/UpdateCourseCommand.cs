using AutoMapper; // اضافه شد
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Courses;
using static Pardis.Domain.Dto.Dtos;
using Pardis.Domain.Categories;

namespace Pardis.Application.Courses.Update
{
    // (بخش Command بدون تغییر)
    public class UpdateCourseCommand : IRequest<CourseResource>
    {
        // ... فیلدها همان قبلی ...
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public long? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Description { get; set; }
        public CourseStatus Status { get; set; }
        public string? InstructorId { get; set; }
        public IFormFile? Image { get; set; }
        public SeoDto? Seo { get; set; }
        public string CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, CourseResource>
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

        public async Task<CourseResource> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repository.GetByIdAsync(request.Id);
            if (course == null) throw new Exception("Course not found");

            Guid oldCategoryId = course.CategoryId;

            // --- منطق بیزینس و آپدیت فیلدها (بدون تغییر) ---
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
            return _mapper.Map<CourseResource>(course);
        }
    }
}