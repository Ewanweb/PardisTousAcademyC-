using Pardis.Domain.Categories;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis.Domain.Seo;

namespace Pardis.Domain.Courses
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public string? Thumbnail { get; set; }
        public CourseStatus Status { get; set; }

        public string InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public User Instructor { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public SeoMetadata Seo { get; set; } = new();

        public bool IsDeleted { get; set; } = false; // Soft Delete Flag
        public DateTime? DeletedAt { get; set; }

        public Course(string title, string slug, string description, long price, string? thumbnail, CourseStatus status, string instructorId, User instructor, int categoryId, Category category, bool isDeleted, DateTime? deletedAt)
        {
            Title = title;
            Slug = slug;
            Description = description;
            Price = price;
            Thumbnail = thumbnail;
            Status = status;
            InstructorId = instructorId;
            Instructor = instructor;
            CategoryId = categoryId;
            Category = category;
            IsDeleted = isDeleted;
            DeletedAt = deletedAt;
        }
    }
}
