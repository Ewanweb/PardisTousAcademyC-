using Pardis.Domain.Dto.Seo;
using Microsoft.AspNetCore.Http;
using Pardis.Domain.Courses;
using System.ComponentModel.DataAnnotations;

namespace Pardis.Domain.Dto.Courses;

public class CreateCourseDto
{
    [Required(ErrorMessage = "عنوان دوره الزامی است.")]
    public string Title { get; set; } = string.Empty;

    public long Price { get; set; }

    [Required(ErrorMessage = "دسته‌بندی دوره الزامی است.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "توضیحات دوره الزامی است.")]
    public string Description { get; set; } = string.Empty;

    public string? StartFrom { get; set; }

    public string? Schedule { get; set; }

    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public CourseType Type { get; set; } = CourseType.Online;

    [Required(ErrorMessage = "محل برگزاری دوره الزامی است.")]
    public string Location { get; set; } = string.Empty;

    public string? InstructorId { get; set; }
    public IFormFile? Image { get; set; }
    public SeoDto? Seo { get; set; }

    [Required(ErrorMessage = "حداقل یک بخش/سرفصل برای دوره الزامی است.")]
    public List<CourseSectionDto> Sections { get; set; } = new();

    public bool IsCompleted { get; set; }
    public bool IsStarted { get; set; }
}