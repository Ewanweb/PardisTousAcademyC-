using Pardis.Domain.Dto.Seo;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared; // ???? IRepository
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;
using System.Linq;
using Pardis.Domain.Dto;
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
            // 1. ?????? ????????? ???? (????? ?? SEO)
            // ??? ?? ??? ??? ?? ??? ??? ?? ?????????? ???? ???? ?? ?? Table ??????? ???????
            var category = await _categoryRepository.Table
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == request.Slug, token);

            if (category == null) return null;

            // 2. ?????? ???? ??????? ???? ???? ???? ??????? (Recursive)
            // ??? ??????? ???? ???? ?? ??????? ?? ??? ????
            var allCategories = await _categoryRepository.Table
                .AsNoTracking()
                .Select(c => new { c.Id, c.ParentId })
                .ToListAsync(token);

            // ???? ????? ???? ????? ??? ???? ? ???? ????????
            var targetIds = new List<Guid> { category.Id };

            // ???????? ???? ???????
            GetRecursiveChildrenIds(category.Id, allCategories.Select(c => (c.Id, c.ParentId)).ToList(), targetIds);

            // 3. ?????? ??????? ?? ???? ???? ????????
            var query = _courseRepository.Table
                .AsNoTracking()
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Schedules)
                .Where(c => targetIds.Contains(c.CategoryId));

            // ????? ????? ??? (??? ???? ???)
            if (!request.IsAdminOrManager)
            {
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            // ????????? (??? ?? ????? ?????)
            // if (request.Page > 0) ...

            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(token);

            // 4. ?????? ?????
            return new
            {
                data = _mapper.Map<List<CourseResource>>(courses),
                category_info = new
                {
                    id = category.Id,
                    title = category.Title,
                    slug = category.Slug,
                    // ??? ?? ????? Seo ?? Owned Type ???
                    description = category.Seo?.MetaDescription,
                    seo = _mapper.Map<SeoDto>(category.Seo)
                }
            };
        }

        // ? ???? ??????? ????? ??? (???? dynamic)
        // ?????: (Id, ParentId) ?? ???? Tuple ?? ????
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
