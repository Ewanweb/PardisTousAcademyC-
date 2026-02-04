using Pardis.Domain.Categories;
using Pardis.Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;
using Pardis.Domain.Seo;

namespace Pardis.Domain.Courses
{
    public class Course : BaseEntity, ISeoEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long Price { get; set; }
        public string? Thumbnail { get; set; }
        public string? StartFrom { get; set; }
        public string Schedule { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }
        public CourseStatus Status { get; set; }
        public CourseType Type { get; set; }
        public string Location { get; set; } = string.Empty;

        public string InstructorId { get; set; } = string.Empty;
        [ForeignKey("InstructorId")]
        public User? Instructor { get; set; }
        public ICollection<UserCourse> Students { get; set; } = new List<UserCourse>();

        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
        
        // ✅ زمان‌های برگذاری دوره
        public ICollection<CourseSchedule> Schedules { get; set; } = new List<CourseSchedule>();

        public SeoMetadata? Seo { get; set; }

        public bool IsDeleted { get; set; } = false; // Soft Delete Flag
        public DateTime? DeletedAt { get; set; }

        // ISeoEntity implementation
        DateTimeOffset ISeoEntity.CreatedAt => new DateTimeOffset(CreatedAt);
        DateTimeOffset ISeoEntity.UpdatedAt => new DateTimeOffset(UpdatedAt);

        public Course() { }

        public Course(
            string title,
            string slug,
            string description,
            long price,
            string? thumbnail,
            CourseStatus status,
            CourseType type,
            string location,
            string instructorId,
            string schedule,
            bool isStarted,
            bool isCompleted,
            string? startFrom,
            Guid categoryId
        )
        {
            Title = title;
            Slug = slug;
            Description = description;
            Price = price;
            Thumbnail = thumbnail;
            Status = status;
            Location = location;
            Type = type;
            InstructorId = instructorId;
            CategoryId = categoryId;
            Seo = null; // Will be set via UpdateSeo method
            Schedule = schedule;
            StartFrom = startFrom;
            IsStarted = isStarted;
            IsCompleted = isCompleted;
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
            return !string.IsNullOrEmpty(Seo?.MetaDescription) ? Seo.MetaDescription : Description;
        }

        public SeoEntityType GetSeoEntityType()
        {
            return SeoEntityType.Course;
        }

        public Dictionary<string, object> GetSeoContext()
        {
            return new Dictionary<string, object>
            {
                ["title"] = Title,
                ["description"] = Description,
                ["price"] = Price,
                ["instructor"] = Instructor?.FullName ?? "نامشخص",
                ["category"] = Category?.Title ?? "عمومی",
                ["type"] = Type.ToString(),
                ["status"] = Status.ToString(),
                ["isStarted"] = IsStarted,
                ["isCompleted"] = IsCompleted,
                ["thumbnail"] = Thumbnail ?? "",
                ["location"] = Location,
                ["schedule"] = Schedule
            };
        }
    }
}
