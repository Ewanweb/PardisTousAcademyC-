using System.ComponentModel.DataAnnotations.Schema;

namespace Pardis.Domain.Blog;

public class PostTag
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }

    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = null!;

    [ForeignKey(nameof(TagId))]
    public Tag Tag { get; set; } = null!;
}
