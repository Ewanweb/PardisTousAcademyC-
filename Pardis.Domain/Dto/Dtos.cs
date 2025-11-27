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
            public int? ParentId { get; set; }
            public bool IsActive { get; set; } = true;
            public IFormFile? Image { get; set; }
            public SeoDto? Seo { get; set; }
        }

        // --- خروجی‌ها (Resources) ---

        public class UserResource
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }

        public class CourseResource
        {
            public int Id { get; set; }
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
        }

        public class CategoryResource
        {
            public int Id { get; set; }
            public string Title { get; set; } // نگاشت Name به Title برای هماهنگی با فرانت
            public string Slug { get; set; }
            public string? Image { get; set; }
            public int? ParentId { get; set; }
            public int CoursesCount { get; set; }
            public string Creator { get; set; } // نام سازنده
            public SeoDto Seo { get; set; }
        }

        public class DashboardStatsDto
        {
            public Dictionary<string, object> Stats { get; set; }
            public List<object> RecentActivity { get; set; }
        }
    }
}
