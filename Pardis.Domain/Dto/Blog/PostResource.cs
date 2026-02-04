using Pardis.Domain.Dto.Seo;
namespace Pardis.Domain.Dto.Blog;

public class PostResource
{
    public Guid Id { get; set; }
    public string UserId { get;  set; } = string.Empty;
    public Guid BlogCategoryId { get;  set; } = Guid.Empty;
    public string Author { get;  set; } = string.Empty;
    public string Title { get;  set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get;  set; } = string.Empty;
    public string SummaryDescription { get;  set; } = string.Empty;
    public string ThumbnailUrl { get;  set; } = string.Empty;
    public SeoDto SeoMetadata { get;  set; } = new SeoDto();
    public long Views { get;  set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } 
}
