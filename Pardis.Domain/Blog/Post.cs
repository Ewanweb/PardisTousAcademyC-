using System.ComponentModel.DataAnnotations.Schema;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;

namespace Pardis.Domain.Blog
{
    public class Post : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public Guid BlogCategoryId { get; set; }

        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        // Legacy fields (kept for backward compatibility)
        public string Description { get; set; } = string.Empty;
        public string SummaryDescription { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;

        // New blog fields
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public DateTime? PublishedAt { get; set; }
        public int ReadingTimeMinutes { get; set; }
        public long Views { get; set; }
        public bool IsDeleted { get; set; }

        public SeoMetadata SeoMetadata { get; set; } = new SeoMetadata();

        #region Relations

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(BlogCategoryId))]
        public virtual BlogCategory BlogCategory { get; set; } = null!;

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
        public ICollection<PostSlugHistory> SlugHistory { get; set; } = new List<PostSlugHistory>();

        #endregion

        public Post() { }

        public Post(
            string title,
            string author,
            string description,
            string summaryDescription,
            string thumbnail,
            string thumbnailUrl,
            SeoMetadata seoMetadata,
            string slug)
        {
            Views = 0;
            Title = title;
            Description = description;
            Content = description;
            Author = author;
            SummaryDescription = summaryDescription;
            Excerpt = summaryDescription;
            Thumbnail = thumbnail;
            ThumbnailUrl = thumbnailUrl;
            CoverImageUrl = thumbnailUrl;
            SeoMetadata = seoMetadata;
            Slug = slug;
        }

        public void Publish(DateTime? publishedAt = null)
        {
            Status = PostStatus.Published;
            PublishedAt = publishedAt ?? DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            Status = PostStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementViews()
        {
            Views += 1;
        }

        public void UpdateReadingTime(int wordsPerMinute = 200)
        {
            var text = Content ?? Description ?? string.Empty;
            var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            ReadingTimeMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / (double)wordsPerMinute));
        }
    }

    public enum PostStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }
}
