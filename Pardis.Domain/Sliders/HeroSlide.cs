using System.ComponentModel.DataAnnotations;

namespace Pardis.Domain.Sliders
{
    public class HeroSlide : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public required string ImageUrl { get; set; }

        [MaxLength(100)]
        public string? ActionLabel { get; set; }

        [MaxLength(500)]
        public string? ActionLink { get; set; }

        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public Guid CreatedByUserId { get; set; }

        private HeroSlide() { }

        public static HeroSlide Create(
            string title,
            string imageUrl,
            Guid createdByUserId,
            string? description = null,
            string? actionLabel = null,
            string? actionLink = null,
            int order = 0)
        {
            var slide = new HeroSlide
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                ImageUrl = imageUrl,
                ActionLabel = actionLabel,
                ActionLink = actionLink,
                Order = order,
                IsActive = true,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return slide;
        }

        public void Update(
            string? title = null,
            string? description = null,
            string? imageUrl = null,
            string? actionLabel = null,
            string? actionLink = null,
            int? order = null,
            bool? isActive = null)
        {
            if (!string.IsNullOrEmpty(title)) Title = title;
            if (description != null) Description = description;
            if (!string.IsNullOrEmpty(imageUrl)) ImageUrl = imageUrl;
            if (actionLabel != null) ActionLabel = actionLabel;
            if (actionLink != null) ActionLink = actionLink;
            if (order.HasValue) Order = order.Value;
            if (isActive.HasValue) IsActive = isActive.Value;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}