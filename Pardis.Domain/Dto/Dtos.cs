using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Pardis.Domain.Courses;

namespace Pardis.Domain.Dto
{
    public class Dtos
    {
        // --- ورودی‌ها (Requests) ---

        public class SeoDto
        {
            public string? MetaTitle { get; set; }
            public string? MetaDescription { get; set; }
            public string? CanonicalUrl { get; set; }
            public bool NoIndex { get; set; }
            public bool NoFollow { get; set; }
        }
        public class AuthResultDto
        {
            public string Token { get; set; }
            public UserResource User { get; set; }
        }

        public class CreateCourseDto
        {
            public string Title { get; set; }
            public long Price { get; set; }
            public Guid CategoryId { get; set; }
            public string Description { get; set; }
            public CourseStatus Status { get; set; }
            public string? InstructorId { get; set; } // اختیاری برای ادمین
            public IFormFile? Image { get; set; } // فایل آپلود
            public SeoDto? Seo { get; set; }
        }

        public class CreateCategoryDto
        {
            public string Name { get; set; }
            public Guid? ParentId { get; set; }
            public bool IsActive { get; set; } = true;
            public IFormFile? Image { get; set; }
            public SeoDto? Seo { get; set; }
        }
        public class RoleDto
        {
            public string Name { get; set; }       // نام انگلیسی (کلید)
            public string Description { get; set; } // عنوان فارسی
        }

        public class UpdateCourseDto
        {
            public string? Title { get; set; }
            public long? Price { get; set; }
            public Guid? CategoryId { get; set; }
            public string? Description { get; set; }
            public CourseStatus? Status { get; set; }
            public string? InstructorId { get; set; }
            public IFormFile? Image { get; set; } 
            public SeoDto? Seo { get; set; }
        }

        // --- خروجی‌ها (Resources) ---

        public class UserResource
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string Mobile { get; set; }
            public string IsActive { get; set; }
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }

        public class CourseResource
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public string Description { get; set; }
            public long Price { get; set; }
            public string Status { get; set; }
            public string Thumbnail { get; set; }

            public UserResource Instructor { get; set; }
            public CategoryResource Category { get; set; }
            public SeoDto Seo { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }

            // تغییر: این فیلد باید نال‌پذیر باشد تا با دیتابیس هماهنگ شود
            public DateTime? DeletedAt { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class CategoryResource
        {
            public Guid Id { get; set; }
            public string Title { get; set; } // نگاشت Name به Title برای هماهنگی با فرانت
            public string Slug { get; set; }
            public string? Image { get; set; }
            public Guid? ParentId { get; set; }
            public int CoursesCount { get; set; }
            public string Creator { get; set; } // نام سازنده
            public SeoDto Seo { get; set; }
            public bool IsActive { get; set; }

        }

        public class DashboardStatsDto
        {
            public Dictionary<string, object> Stats { get; set; }
            public List<object> RecentActivity { get; set; }
        }
        public class RecentActivityDto
        {
            public string Id { get; set; }
            public string Type { get; set; }   // "course", "user", "category"
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public DateTime Time { get; set; }
        }

        public class CategoryChildrenDto
        {
            public CategoryResource Parent { get; set; }
            public List<CategoryResource> Children { get; set; }
        }
        public class CategoryWithCountDto
        {
            public Pardis.Domain.Categories.Category Category { get; set; }
            public int CoursesCount { get; set; }
        }
    }
}
