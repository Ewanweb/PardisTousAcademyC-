using System;
using System.Collections.Generic;
using System.Text;

namespace Pardis.Application.Courses
{
    internal class CourseService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CourseService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Course> CreateCourseAsync(CreateCourseDto dto, User currentUser, bool isAdmin)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. آپلود تصویر
                string? imagePath = await UploadImageAsync(dto.Image);

                // 2. تعیین مدرس
                string instructorId = currentUser.Id;
                if (isAdmin && !string.IsNullOrEmpty(dto.InstructorId))
                {
                    instructorId = dto.InstructorId;
                }

                // 3. ساخت اسلاگ
                string slug = dto.Title.Replace(" ", "-").ToLower() + "-" + DateTime.Now.Ticks;

                var course = new Course
                {
                    Title = dto.Title,
                    Slug = slug,
                    Description = dto.Description,
                    Price = dto.Price,
                    CategoryId = dto.CategoryId,
                    InstructorId = instructorId,
                    Status = dto.Status,
                    Thumbnail = imagePath,
                    Seo = new SeoMetadata
                    {
                        MetaTitle = dto.Seo?.MetaTitle,
                        MetaDescription = dto.Seo?.MetaDescription,
                        CanonicalUrl = dto.Seo?.CanonicalUrl,
                        NoIndex = dto.Seo?.NoIndex ?? false,
                        NoFollow = dto.Seo?.NoFollow ?? false
                    }
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                // 4. آپدیت شمارنده دسته‌بندی
                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category != null)
                {
                    category.CoursesCount++;
                    _context.Categories.Update(category);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return course;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SoftDeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return;

            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;

            // کاهش شمارنده
            var category = await _context.Categories.FindAsync(course.CategoryId);
            if (category != null)
            {
                category.CoursesCount = Math.Max(0, category.CoursesCount - 1);
            }

            await _context.SaveChangesAsync();
        }

        // متد کمکی آپلود فایل
        private async Task<string?> UploadImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // بازگرداندن لینک کامل
            return $"/uploads/{fileName}";
        }

        // متدهای Update, Restore, ForceDelete مشابه لاراول پیاده‌سازی می‌شوند
    }
}
