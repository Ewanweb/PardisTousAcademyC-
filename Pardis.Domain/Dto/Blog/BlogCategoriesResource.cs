namespace Pardis.Domain.Dto.Blog;

public class BlogCategoriesResource
{
    public Guid Id { get; set; }
    public string UserId { get;  set; } = string.Empty;
    public string CreatedBy { get;  set; } = string.Empty;
    public string Title { get;  set; } = string.Empty;
    public string Slug { get;  set; } = string.Empty;
    public string Thumbnail { get;  set; } = string.Empty;
    public string ThumbnailUrl { get;  set; } = string.Empty;
    public SeoDto SeoMetadata { get; set; } = new SeoDto();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}