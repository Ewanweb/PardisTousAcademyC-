using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;

namespace Pardis.Domain.Blog
{
    public class BlogCategory : BaseEntity
    {
        public string UserId { get;  set; } 
        public string CreatedBy { get;  set; } = string.Empty;
        public string Title { get;  set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Thumbnail { get;  set; } = string.Empty;
        public string ThumbnailUrl { get;  set; } = string.Empty;
        public SeoMetadata SeoMetadata { get; set; } = new();

        #region Relations

        [ForeignKey(nameof(UserId))]
        public virtual User User { get;  set; }
        public ICollection<Post> Posts { get;  set; }

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
