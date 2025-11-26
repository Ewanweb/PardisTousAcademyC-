using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;

namespace Pardis.Domain.Categories
{
    public class Category : BaseEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public int CoursesCount { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public User? Creator { get; set; }

        public ICollection<Course> Courses { get; set; }

        public SeoMetadata Seo { get; set; } = new(); // Owned Type

        public Category(string title, string slug, string? image, int? parentId, Category? parent, ICollection<Category> children, string? createdById, User? creator, ICollection<Course> courses)
        {
            Title = title;
            Slug = slug;
            Image = image;
            IsActive = true;
            CoursesCount = 0;
            ParentId = parentId;
            Parent = parent;
            Children = children;
            CreatedById = createdById;
            Creator = creator;
            Courses = courses;
        }
    }
}
