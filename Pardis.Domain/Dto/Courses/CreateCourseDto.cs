using Microsoft.AspNetCore.Http;
using Pardis.Domain.Courses;
using System.ComponentModel.DataAnnotations;

namespace Pardis.Domain.Dto.Courses;

public class CreateCourseDto
{
    [Required(ErrorMessage = "عنوان دوره الزامی است")]
    public string Title { get; set; } = string.Empty;
    
    public long Price { get; set; }
    
    [Required(ErrorMessage = "دسته‌بندی دوره الزامی است")]
    public Guid CategoryId { get; set; }
    
    [Required(ErrorMessage = "توضیحات دوره الزامی است")]
    public string Description { get; set; } = string.Empty;
    
    public string? StartFrom { get; set; }
    
    [Required(ErrorMessage = "زمان‌بندی دوره الزامی است")]
    public string Schedule { get; set; } = string.Empty;
    
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public CourseType Type { get; set; } = CourseType.Online;
    
    [Required(ErrorMessage = "محل برگزاری دوره الزامی است")]
    public string Location { get; set; } = string.Empty;
    
    public string? InstructorId { get; set; } // اختیاری برای ادمین
    public IFormFile? Image { get; set; } // فایل آپلود
    public SeoDto? Seo { get; set; }
    
    [Required(ErrorMessage = "حداقل یک بخش برای دوره الزامی است")]
    public List<CourseSectionDto> Sections { get; set; } = new();
    
    public bool IsCompleted { get; set; }
    public bool IsStarted { get; set; }
}