using Microsoft.AspNetCore.Http;
using Pardis.Domain.Courses;

namespace Pardis.Domain.Dto.Courses;

public class CreateCourseDto
{
    public string Title { get; set; }
    public long Price { get; set; }
    public Guid CategoryId { get; set; }
    public string Description { get; set; }
    public string? StartFrom { get; set; }
    public string Schedule { get; set; }
    public CourseStatus Status { get; set; }
    public CourseType Type { get; set; }
    public string Location { get; set; }
    public string? InstructorId { get; set; } // اختیاری برای ادمین
    public IFormFile? Image { get; set; } // فایل آپلود
    public Dtos.SeoDto? Seo { get; set; }
    public List<CourseSectionDto> Sections { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsStarted { get; set; }
}