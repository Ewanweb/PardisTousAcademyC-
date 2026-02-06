namespace Pardis.Domain.Dto.Blog;

public class CreatePostRequestDto
{
    public Guid BlogCategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public Pardis.Domain.Dto.Seo.SeoDto Seo { get; set; } = new();
}

public class UpdatePostRequestDto : CreatePostRequestDto
{
}

public class CreateCategoryRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public string? CoverImageUrl { get; set; }
    public Pardis.Domain.Dto.Seo.SeoDto Seo { get; set; } = new();
}

public class UpdateCategoryRequestDto : CreateCategoryRequestDto
{
}

public class CreateTagRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class UpdateTagRequestDto : CreateTagRequestDto
{
}
