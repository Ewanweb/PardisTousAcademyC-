using System.ComponentModel.DataAnnotations;
using Pardis.Domain.Seo;

namespace Pardis.Domain.Pages
{
    public class Page : BaseEntity, ISeoEntity
    {
        [MaxLength(255)]
        public string Slug { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Content { get; set; }
        
        [MaxLength(50)]
        public string PageType { get; set; } = "Static";
        
        public bool IsPublished { get; set; } = true;
        
        [MaxLength(2)]
        public string Language { get; set; } = "fa";
        
        public SeoMetadata Seo { get; set; } = new();

        public Page() { }

        public Page(string slug, string title, string? description = null, string? content = null, 
                   string pageType = "Static", bool isPublished = true, string language = "fa")
        {
            Slug = slug;
            Title = title;
            Description = description;
            Content = content;
            PageType = pageType;
            IsPublished = isPublished;
            Language = language;
            Seo = new SeoMetadata();
        }

        // ISeoEntity implementation
        DateTimeOffset ISeoEntity.CreatedAt => new DateTimeOffset(CreatedAt);
        DateTimeOffset ISeoEntity.UpdatedAt => new DateTimeOffset(UpdatedAt);

        public string GetSeoTitle()
        {
            return !string.IsNullOrEmpty(Seo.MetaTitle) ? Seo.MetaTitle : Title;
        }

        public string GetSeoDescription()
        {
            return !string.IsNullOrEmpty(Seo.MetaDescription) ? Seo.MetaDescription : 
                   Description ?? $"{Title} - آکادمی پردیس توس";
        }

        public SeoEntityType GetSeoEntityType()
        {
            return SeoEntityType.Page;
        }

        public Dictionary<string, object> GetSeoContext()
        {
            return new Dictionary<string, object>
            {
                ["title"] = Title,
                ["description"] = Description ?? "",
                ["pageType"] = PageType,
                ["isPublished"] = IsPublished,
                ["language"] = Language,
                ["hasContent"] = !string.IsNullOrEmpty(Content)
            };
        }
    }
}
