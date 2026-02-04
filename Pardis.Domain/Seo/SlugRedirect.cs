using System.ComponentModel.DataAnnotations;

namespace Pardis.Domain.Seo
{
    public class SlugRedirect
    {
        public int Id { get; set; }
        
        [MaxLength(255)]
        public string OldSlug { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string NewSlug { get; set; } = string.Empty;
        
        public SeoEntityType EntityType { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
        
        public bool IsActive { get; set; } = true;

        public SlugRedirect() { }

        public SlugRedirect(string oldSlug, string newSlug, SeoEntityType entityType)
        {
            OldSlug = oldSlug;
            NewSlug = newSlug;
            EntityType = entityType;
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }
    }
}