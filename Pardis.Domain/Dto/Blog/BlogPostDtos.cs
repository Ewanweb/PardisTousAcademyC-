using Pardis.Domain.Dto.Seo;

namespace Pardis.Domain.Dto.Blog;

public class PostListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Author { get; set; } = string.Empty;
    public string CategoryTitle { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public long ViewCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? HighlightedTitle { get; set; }
    public string? HighlightedExcerpt { get; set; }
}

public class PostDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Author { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public long ViewCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public BlogCategoryDto Category { get; set; } = new();
    public List<TagDto> Tags { get; set; } = new();
    public SeoDto Seo { get; set; } = new();
}

public class BlogCategoryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public SeoDto Seo { get; set; } = new();
}

public class TagDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class PostNavDto
{
    public PostListItemDto? Next { get; set; }
    public PostListItemDto? Previous { get; set; }
}

public class PostSlugResolveDto
{
    public PostDetailDto? Post { get; set; }
    public bool IsRedirect { get; set; }
    public string? RedirectSlug { get; set; }
}
