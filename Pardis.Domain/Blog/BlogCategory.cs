using System.ComponentModel.DataAnnotations.Schema;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;

namespace Pardis.Domain.Blog
{
    public class BlogCategory : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public SeoMetadata SeoMetadata { get; set; } = new();

        #region Relations

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        #endregion
        public BlogCategory() { }

        public BlogCategory(string title, string thumbnail, string thumbnailUrl, SeoMetadata seoMetadata, string createdBy, string slug)
        {
            Thumbnail = thumbnail;
            ThumbnailUrl = thumbnailUrl;
            SeoMetadata = seoMetadata;
            Title = title;
            CreatedBy = createdBy;
            Slug = slug;
        }
    }
}
