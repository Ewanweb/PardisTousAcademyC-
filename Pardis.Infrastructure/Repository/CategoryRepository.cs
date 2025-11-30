using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Infrastructure.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Category?> GetCategoryWithIdWithSeo(Guid id, CancellationToken token)
        {
            var category = await _context.Categories.Include(c => c.Seo).FirstOrDefaultAsync(c => c.Id == id, token);

            return category;
            
        }

        public async Task<List<Category>?> GetCategories()
        {
            var query = _context.Categories
                .Include(c => c.Seo)
                .AsQueryable();

            return await query.Include(c => c.Children).ThenInclude(c => c.Seo).ToListAsync();

        }

        public async Task<bool> IsExist(string title)
        {
            return await _context.Categories.AnyAsync(c => c.Title == title);
        }

        public async Task<int> MoveCategoryForDelete(Guid id, Guid migrateToId, CancellationToken token)
        {
            return await _context.Categories
            .Where(c => c.ParentId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Id, migrateToId), token);
        }

        public async Task<bool> SlugIsExist(string slug)
        {
            return await _context.Categories.AnyAsync(c => c.Slug == slug);
        }

        public async Task<Category?> GetCategoryById(Guid id, CancellationToken token)
        {
            return await _context.Categories
                .Include(c => c.Seo)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }

        public Task<Category?> GetCategoryChildren(Guid id, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CategoryWithCountDto>> GetChildrenWithCourseCountAsync(Guid parentId, CancellationToken token)
        {
            var result = await _context.Categories
                .Where(c => c.ParentId == parentId)
                .Include(c => c.Seo)
                // Select برای Count کردن دوره‌ها
                .Select(c => new CategoryWithCountDto
                {
                    Category = c,
                    CoursesCount = c.Courses.Count()
                })
                .OrderByDescending(c => c.Category.CreatedAt)
                .ToListAsync(token);

            return result;
        }
    }
}
