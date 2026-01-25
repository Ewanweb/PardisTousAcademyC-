using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Users;

namespace Pardis.Domain.Dto.Courses;

public class CourseResource
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

    // ✅ تغییر: فقط اطلاعات ضروری Instructor
    public required InstructorBasicDto Instructor { get; set; }
    public required CategoryResource Category { get; set; }
    public List<CourseSectionDto> Sections { get; set; } = [];
    
    // ✅ زمان‌های برگذاری دوره
    public List<CourseScheduleDto> Schedules { get; set; } = [];

    public required SeoDto Seo { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public required bool IsDeleted { get; set; }
}

/// <summary>
/// DTO ساده برای Instructor - بدون Courses برای جلوگیری از circular reference
/// </summary>
public class InstructorBasicDto
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Mobile { get; set; }
}