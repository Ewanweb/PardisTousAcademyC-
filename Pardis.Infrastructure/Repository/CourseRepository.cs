using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Infrastructure.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly AppDbContext _context;
        public CourseRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> MoveCoursesForDelete(Guid categoryId, Guid migrateToId, CancellationToken token)
        {
            return await _context.Courses
                        .Where(c => c.CategoryId == categoryId)
                        .ExecuteUpdateAsync(s => s.SetProperty(c => c.CategoryId, migrateToId), token);
        }

        public async Task<Course?> GetDeletedCourseById(Guid id, CancellationToken token)
        {
            return await _context.Courses
                        .IgnoreQueryFilters()
                        .Include(c => c.Seo)
                        .FirstOrDefaultAsync(c => c.Id == id, token);
        }

        public async Task<List<Course>> GetCoursesWithFilterAsync(bool trashed, bool isAdminOrManager, Guid? categoryId, CancellationToken token)
        {
            var query = _context.Courses.AsQueryable();

            // فیلتر Soft Delete
            if (trashed)
            {
                // اگر درخواست برای سطل زباله باشد، فیلتر گلوبال را نادیده بگیر و فقط حذف شده‌ها را بیاور
                query = query.IgnoreQueryFilters().Where(c => c.IsDeleted);
            }
            else
            {
                // در غیر این صورت فقط رکوردهای سالم (که IsDeleted == false هستند)
                query = query.Where(c => !c.IsDeleted);
            }

            // فیلتر سطح دسترسی
            if (!isAdminOrManager)
            {
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            // فیلتر دسته‌بندی تک
            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            return await query
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Seo)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(token);
        }

        // 2. پیاده‌سازی دریافت تکی با جزئیات کامل
        public async Task<Course?> GetCourseByIdWithDetailsAsync(Guid id, CancellationToken token)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Seo)
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }

        // 3. پیاده‌سازی دریافت بر اساس لیست دسته‌ها (برای نمایش درختی)
        public async Task<List<Course>> GetCoursesByCategoryListAsync(List<Guid> categoryIds, bool isAdminOrManager, CancellationToken token)
        {
            var query = _context.Courses
                .Where(c => categoryIds.Contains(c.CategoryId)) // شرط اصلی: آیا در این لیست IDها هست؟
                .AsQueryable();

            if (!isAdminOrManager)
            {
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            return await query
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Seo)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(token);
        }

    }
}
