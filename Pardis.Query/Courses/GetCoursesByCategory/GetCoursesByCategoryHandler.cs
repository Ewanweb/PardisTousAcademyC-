using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared; // برای IRepository
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;
using System.Linq;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCoursesByCategory
{
    public class GetCoursesByCategoryHandler : IRequestHandler<GetCoursesByCategoryQuery, object>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public GetCoursesByCategoryHandler(
            IRepository<Course> courseRepository,
            IRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetCoursesByCategoryQuery request, CancellationToken token)
        {
            // 1. دریافت دسته‌بندی اصلی (همراه با SEO)
            // فرض بر این است که این متد در ریپازیتوری وجود دارد یا از Table استفاده می‌کنیم
            var category = await _categoryRepository.Table
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == request.Slug, token);

            if (category == null) return null;

            // 2. دریافت تمام دسته‌ها برای پیدا کردن فرزندان (Recursive)
            // فقط فیلدهای مورد نیاز را می‌کشیم تا سبک باشد
            var allCategories = await _categoryRepository.Table
                .AsNoTracking()
                .Select(c => new { c.Id, c.ParentId })
                .ToListAsync(token);

            // لیست نهایی شامل آی‌دی خود دسته و تمام فرزندانش
            var targetIds = new List<Guid> { category.Id };

            // فراخوانی تابع بازگشتی
            GetRecursiveChildrenIds(category.Id, allCategories.Select(c => (c.Id, c.ParentId)).ToList(), targetIds);

            // 3. دریافت دوره‌ها بر اساس لیست آی‌دی‌ها
            var query = _courseRepository.Table
                .AsNoTracking()
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Schedules)
                .Where(c => targetIds.Contains(c.CategoryId));

            // اعمال فیلتر نقش (اگر لازم است)
            if (!request.IsAdminOrManager)
            {
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            // صفحه‌بندی (اگر در کوئری دارید)
            // if (request.Page > 0) ...

            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(token);

            // 4. بازگشت خروجی
            return new
            {
                data = _mapper.Map<List<CourseResource>>(courses),
                category_info = new
                {
                    id = category.Id,
                    title = category.Title,
                    slug = category.Slug,
                    // فرض بر اینکه Seo یک Owned Type است
                    description = category.Seo?.MetaDescription,
                    seo = _mapper.Map<SeoDto>(category.Seo)
                }
            };
        }

        // ✅ تابع بازگشتی اصلاح شده (بدون dynamic)
        // ورودی: (Id, ParentId) به صورت Tuple یا کلاس
        private void GetRecursiveChildrenIds(Guid parentId, List<(Guid Id, Guid? ParentId)> allCats, List<Guid> result)
        {
            var childrenIds = allCats
                .Where(c => c.ParentId == parentId)
                .Select(c => c.Id)
                .ToList();

            if (childrenIds.Any())
            {
                result.AddRange(childrenIds);
                foreach (var childId in childrenIds)
                {
                    GetRecursiveChildrenIds(childId, allCats, result);
                }
            }
        }
    }
}