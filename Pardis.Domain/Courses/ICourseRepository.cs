using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Domain.Courses
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<int> MoveCoursesForDelete(Guid categoryId, Guid migrateToId, CancellationToken token);
        Task<Course?> GetDeletedCourseById(Guid id, CancellationToken token);
        // 1. لیست دوره‌ها با فیلترهای کامل (ادمین، حذف شده، دسته‌بندی خاص)
        Task<List<Course>> GetCoursesWithFilterAsync(bool trashed, bool isAdminOrManager, Guid? categoryId, CancellationToken token);

        // 2. دریافت جزئیات کامل یک دوره (برای صفحه نمایش تکی)
        Task<Course?> GetCourseByIdWithDetailsAsync(Guid id, CancellationToken token);

        // 3. دریافت دوره‌هایی که در لیست دسته‌بندی‌های داده شده هستند (برای نمایش درختی)
        Task<List<Course>> GetCoursesByCategoryListAsync(List<Guid> categoryIds, bool isAdminOrManager, CancellationToken token);

    }
}
