using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users; // مطمئن شو Dto ها ایمپورت شده باشن
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourses
{
    public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, List<CourseResource>>
    {
        private readonly IRepository<Course> _repository;
        // private readonly IMapper _mapper; // اگر دستی مپ میکنی نیازی به این نیست

        public GetCoursesHandler(IRepository<Course> repository)
        {
            _repository = repository;
        }

        public async Task<List<CourseResource>> Handle(GetCoursesQuery request, CancellationToken token)
        {
            // 1. دسترسی مستقیم به IQueryable (هنوز درخواستی به دیتابیس زده نشده)
            // فرض بر این است که _repository.Table یک IQueryable برمی‌گرداند
            var query = _repository.Table
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Sections)
                .Include(c => c.Seo)
                .AsNoTracking() // برای سرعت بیشتر (چون فقط خواندنی است)
                .AsQueryable();

            // 2. اعمال فیلترها (این‌ها تبدیل به WHERE در SQL می‌شوند)

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
                // ادمین همه را می‌بیند (بدون فیلتر)
            }
            else if (request.IsInstructor && !string.IsNullOrEmpty(request.CurrentUserId))
            {
                // مدرس فقط دوره‌های خودش
                query = query.Where(c => c.InstructorId == request.CurrentUserId);
            }
            else
            {
                // کاربر عادی فقط منتشر شده‌ها
                // نکته: اگر Status اینام است، مطمئن شو در دیتابیس درست ذخیره شده
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            // 3. اجرا و دریافت از دیتابیس (اینجا کوئری زده می‌شود)
            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(token);

            // 4. تبدیل به Resource (در حافظه)
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

                Instructor = c.Instructor != null ? new InstructorBasicDto
                {
                    Id = c.Instructor.Id,
                    FullName = c.Instructor.FullName ?? c.Instructor.UserName ?? "",
                    Email = c.Instructor.Email ?? "",
                    Mobile = c.Instructor.PhoneNumber
                } : null!,

                Category = c.Category != null ? new CategoryResource
                {
                    Id = c.Category.Id,
                    Title = c.Category.Title,
                    Slug = c.Category.Slug,
                    CoursesCount = c.Category.CoursesCount
                } : null!,

                Sections = c.Sections != null
                    ? c.Sections.OrderBy(s => s.Order).Select(s => new CourseSectionDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description,
                        Order = s.Order
                    }).ToList()
                    : new List<CourseSectionDto>(),

                Seo = c.Seo != null ? new SeoDto
                {
                    MetaTitle = c.Seo.MetaTitle,
                    MetaDescription = c.Seo.MetaDescription,
                    CanonicalUrl = c.Seo.CanonicalUrl,
                    NoIndex = c.Seo.NoIndex,
                    NoFollow = c.Seo.NoFollow
                } : new SeoDto(),

            }).ToList(); // ✅ تبدیل نهایی به لیست

            return result;
        }
    }
}