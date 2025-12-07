using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Users;

namespace Pardis.Domain.Dto.Courses;

public class CourseResource
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public long Price { get; set; }
    public string Status { get; set; }
    public string? StartFrom { get; set; }
    public string Schedule { get; set; }
    public string Thumbnail { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsStarted { get; set; }

    public UserResource Instructor { get; set; }
    public CategoryResource Category { get; set; }
    public List<CourseSectionDto> Sections { get; set; } = new();

    public Dtos.SeoDto Seo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // تغییر: این فیلد باید نال‌پذیر باشد تا با دیتابیس هماهنگ شود
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}