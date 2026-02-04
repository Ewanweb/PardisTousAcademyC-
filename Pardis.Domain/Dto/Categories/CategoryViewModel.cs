using Pardis.Domain.Dto.Seo;

namespace Pardis.Domain.Dto.Categories;

/// <summary>
/// Category view model for MVC views
/// </summary>
public class CategoryViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public bool IsActive { get; set; }
    public int CoursesCount { get; set; }
    
    public Guid? ParentId { get; set; }
    public string? ParentTitle { get; set; }
    
    public List<CategoryViewModel> Children { get; set; } = new();
    public List<CourseViewModel> Courses { get; set; } = new();
    
    public SeoDto? Seo { get; set; }

    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Simplified course view model for category display
/// </summary>
public class CourseViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Price { get; set; }
    public string? Thumbnail { get; set; }
    public bool IsActive { get; set; }
}
