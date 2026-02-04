using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Users;

namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// DTO برای نمایش جزئیات کامل دوره با اطلاعات دسترسی
/// </summary>
public class CourseDetailDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required long Price { get; set; }
    public required string Status { get; set; }
    public required string Type { get; set; }
    public required string Location { get; set; }
    public string? StartFrom { get; set; }
    public required string Schedule { get; set; }
    public required string Thumbnail { get; set; }
    public required bool IsCompleted { get; set; }
    public required bool IsStarted { get; set; }

    // اطلاعات مربوطه
    public required InstructorBasicDto Instructor { get; set; }
    public required CategoryResource Category { get; set; }
    public List<CourseSectionDto> Sections { get; set; } = [];
    public List<CourseScheduleDto> Schedules { get; set; } = [];
    public required SeoDto Seo { get; set; }

    // وضعیت کاربر
    public required bool IsPurchased { get; set; }
    public required bool IsInCart { get; set; }

    /// <summary>
    /// اطلاعات دسترسی - فقط برای کاربران خریدار پر می‌شود
    /// </summary>
    public CourseAccessDto? Access { get; set; }

    // تاریخ‌ها
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public required bool IsDeleted { get; set; }
}

