using Pardis.Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;

namespace Pardis.Domain.Categories
{
    public class Category : BaseEntity, ISeoEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CoursesCount { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        public string? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public User? Creator { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();

        public SeoMetadata? Seo { get; set; } // Owned Type

        // ISeoEntity implementation
        DateTimeOffset ISeoEntity.CreatedAt => new DateTimeOffset(CreatedAt);
        DateTimeOffset ISeoEntity.UpdatedAt => new DateTimeOffset(UpdatedAt);

        private Category()
        {

        }
        
        public Category(string title, string slug, string? image, Guid? parentId, string? createdById, string? description = null)
        {
            Title = title;
            Slug = slug;
            Image = image;
            Description = description;
            IsActive = true;
            CoursesCount = 0;
            ParentId = parentId;
            CreatedById = createdById;
            Children = new List<Category>();
            Courses = new List<Course>();
            Seo = null; // Will be set via UpdateSeo method
        }

        public void UpdateSeo(SeoMetadata seoMetadata)
        {
            Seo = seoMetadata;
        }

        // ISeoEntity implementation
        public string GetSeoTitle()
        {
            return !string.IsNullOrEmpty(Seo?.MetaTitle) ? Seo.MetaTitle : Title;
        }

        public string GetSeoDescription()
        {
            return !string.IsNullOrEmpty(Seo?.MetaDescription) ? Seo.MetaDescription : 
                   Description ?? $"دوره‌های آموزشی {Title} - {CoursesCount} دوره موجود";
        }

        public SeoEntityType GetSeoEntityType()
        {
            return SeoEntityType.Category;
        }

        public Dictionary<string, object> GetSeoContext()
        {
            return new Dictionary<string, object>
            {
                ["title"] = Title,
                ["coursesCount"] = CoursesCount,
                ["isActive"] = IsActive,
                ["hasParent"] = ParentId.HasValue,
                ["parentTitle"] = Parent?.Title ?? "",
                ["childrenCount"] = Children.Count,
                ["image"] = Image ?? ""
            };
        }
    }
}
