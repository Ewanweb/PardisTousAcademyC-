using System.ComponentModel.DataAnnotations.Schema;

namespace Pardis.Domain.Blog;

public class PostSlugHistory : BaseEntity
{
    public Guid PostId { get; set; }
    public string OldSlug { get; set; } = string.Empty;
    public string NewSlug { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = null!;
}
