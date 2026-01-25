using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;

namespace Pardis.Domain.Blog
{
    public class Post : BaseEntity
    {
        public string UserId { get;  set; } 
        public Guid BlogCategoryId { get;  set; } 
        public string Author { get;  set; } 
        public string Title { get;  set; } 
        public string Slug { get; set; } 
        public string Description { get;  set; } 
        public string SummaryDescription { get;  set; } 
        public string Thumbnail { get;  set; } 
        public string ThumbnailUrl { get;  set; }
        public SeoMetadata SeoMetadata { get; set; } = new SeoMetadata();
        public long Views { get;  set; }

        #region Relations

        [ForeignKey(nameof(UserId))]
        public virtual User User { get;  set; }


        [ForeignKey(nameof(BlogCategoryId))]
        public virtual BlogCategory BlogCategory { get;  set; }

        #endregion


        public Post()
        {
            
        }

        public Post(string title, string author,string description, string summaryDescription, string thumbnail, string thumbnailUrl, SeoMetadata seoMetadata, string slug)
        {
            Views = 0;
            Title = title;
            Description = description;
            Author = author;
            SummaryDescription = summaryDescription;
            Thumbnail = thumbnail;
            ThumbnailUrl = thumbnailUrl;
            SeoMetadata = seoMetadata;
            Slug = slug;

        }
    }
}
