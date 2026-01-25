using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourses
{
    public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, List<CourseResource>>
    {
        private readonly IRepository<Course> _repository;

        public GetCoursesHandler(IRepository<Course> repository)
        {
            _repository = repository;
        }

        public async Task<List<CourseResource>> Handle(GetCoursesQuery request, CancellationToken token)
        {
            // ✅ بهینه‌سازی: برای صفحه اصلی فقط اطلاعات ضروری
            var query = _repository.Table
                .Include(c => c.Instructor) // فقط instructor
                .Include(c => c.Category)   // فقط category
                .AsNoTracking() // برای سرعت بیشتر
                .AsQueryable();

            // فیلتر سطل زباله
            if (request.Trashed)
            {
                query = query.IgnoreQueryFilters().Where(c => c.IsDeleted);
            }
            else
            {
                query = query.Where(c => !c.IsDeleted);
            }

            // فیلتر دسته‌بندی
            if (request.CategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == request.CategoryId);
            }

            // فیلتر نقش‌ها
            if (request.IsAdminOrManager)
            {
                // ادمین همه را می‌بیند
            }
            else if (request.IsInstructor && !string.IsNullOrEmpty(request.CurrentUserId))
            {
                // مدرس فقط دوره‌های خودش
                query = query.Where(c => c.InstructorId == request.CurrentUserId);
            }
            else
            {
                // کاربر عادی فقط منتشر شده‌ها
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            // ✅ بهینه‌سازی: صفحه‌بندی برای بهتر شدن performance
            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(token);

            // ✅ تبدیل ساده به Resource (بدون اطلاعات اضافی)
            var result = courses.Select(c => new CourseResource
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                Description = c.Description,
                Price = c.Price,
                Status = c.Status.ToString(),
                Type = c.Type.ToString(),
                Location = c.Location,
                Thumbnail = c.Thumbnail ?? "",
                StartFrom = c.StartFrom,
                Schedule = c.Schedule,
                IsCompleted = c.IsCompleted,
                IsStarted = c.IsStarted,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsDeleted = c.IsDeleted,

                // ✅ فقط اطلاعات ضروری instructor
                Instructor = c.Instructor != null ? new InstructorBasicDto
                {
                    Id = c.Instructor.Id,
                    FullName = c.Instructor.FullName ?? c.Instructor.UserName ?? "",
                    Email = c.Instructor.Email ?? "",
                    Mobile = c.Instructor.PhoneNumber
                } : null,

                // ✅ فقط اطلاعات ضروری category
                Category = c.Category != null ? new CategoryResource
                {
                    Id = c.Category.Id,
                    Title = c.Category.Title,
                    Slug = c.Category.Slug,
                    CoursesCount = c.Category.CoursesCount
                } : null,

                // ✅ برای صفحه اصلی این‌ها لازم نیست - empty lists
                Sections = new List<CourseSectionDto>(),
                Seo = new SeoDto(),
                Schedules = new List<CourseScheduleDto>()

            }).ToList();

            return result;
        }
    }
}