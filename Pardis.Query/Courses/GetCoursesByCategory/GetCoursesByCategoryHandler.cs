using AutoMapper;
using MediatR;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;
using Pardis.Infrastructure.Repository;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCoursesByCategory
{
    public class GetCoursesByCategoryHandler : IRequestHandler<GetCoursesByCategoryQuery, object>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository; // تزریق ریپازیتوری دسته
        private readonly IMapper _mapper;

        public GetCoursesByCategoryHandler(
            ICourseRepository courseRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetCoursesByCategoryQuery request, CancellationToken token)
        {
            // 1. دریافت اطلاعات دسته بندی اصلی (برای نمایش تایتل و سئو در بالای صفحه)
            var category = await _categoryRepository.GetCategoryWithIdWithSeo(request.Slug, token);
            if (category == null) return null;

            // 2. دریافت تمام دسته‌ها برای پیدا کردن فرزندان (Recursive)
            // اصلاح: ابتدا لیست را کامل دریافت می‌کنیم، سپس در حافظه Select می‌زنیم
            var categoriesList = await _categoryRepository.GetCategories();

            var allCategories = (categoriesList ?? new List<Category>())
                .Select(c => new { c.Id, c.ParentId })
                .ToList();

            var targetIds = new List<Guid> { category.Id };
            GetRecursiveChildrenIds(category.Id, allCategories, targetIds);

            // 3. دریافت دوره‌ها از طریق ریپازیتوری کورس
            var courses = await _courseRepository.GetCoursesByCategoryListAsync(
                targetIds,
                request.IsAdminOrManager,
                token
            );

            // 4. بازگشت خروجی
            return new
            {
                data = _mapper.Map<List<CourseResource>>(courses),
                category_info = new
                {
                    id = category.Id,
                    title = category.Title,
                    slug = category.Slug,
                    description = category.Seo?.MetaDescription,
                    seo = category.Seo,
                }
            };
        }

        private void GetRecursiveChildrenIds(Guid parentId, IEnumerable<dynamic> allCats, List<Guid> result)
        {
            var childrenIds = allCats
                .Where(c => c.ParentId == parentId)
                .Select(c => (Guid)c.Id);

            foreach (var childId in childrenIds)
            {
                result.Add(childId);
                GetRecursiveChildrenIds(childId, allCats, result);
            }
        }
    }
}
