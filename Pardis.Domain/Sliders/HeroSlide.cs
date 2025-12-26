using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
        public string? Badge { get; set; }

        // Primary Action
        [MaxLength(100)]
        public string? PrimaryActionLabel { get; set; }

        [MaxLength(500)]
        public string? PrimaryActionLink { get; set; }

        // Secondary Action
        [MaxLength(100)]
        public string? SecondaryActionLabel { get; set; }

        [MaxLength(500)]
        public string? SecondaryActionLink { get; set; }

        // Stats stored as JSON
        public string? StatsJson { get; set; }

        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsPermanent { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }

        // Navigation Properties
        public Guid CreatedByUserId { get; set; }

        // Legacy properties for backward compatibility
        [MaxLength(500)]
        public string? LinkUrl 
        { 
            get => PrimaryActionLink; 
            set => PrimaryActionLink = value; 
        }

        [MaxLength(100)]
        public string? ButtonText 
        { 
            get => PrimaryActionLabel; 
            set => PrimaryActionLabel = value; 
        }

        private HeroSlide() { }

        public static HeroSlide Create(
            string title,
            string imageUrl,
            Guid createdByUserId,
            string? description = null,
            string? badge = null,
            string? primaryActionLabel = null,
            string? primaryActionLink = null,
            string? secondaryActionLabel = null,
            string? secondaryActionLink = null,
            string? statsJson = null,
            int order = 0,
            bool isPermanent = true,
            DateTime? expiresAt = null)
        {
            var slide = new HeroSlide
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                ImageUrl = imageUrl,
                Badge = badge,
                PrimaryActionLabel = primaryActionLabel,
                PrimaryActionLink = primaryActionLink,
                SecondaryActionLabel = secondaryActionLabel,
                SecondaryActionLink = secondaryActionLink,
                StatsJson = statsJson,
                Order = order,
                IsActive = true,
                IsPermanent = isPermanent,
                ExpiresAt = isPermanent ? null : expiresAt ?? DateTime.UtcNow.AddHours(24),
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
            string? badge = null,
            string? primaryActionLabel = null,
            string? primaryActionLink = null,
            string? secondaryActionLabel = null,
            string? secondaryActionLink = null,
            string? statsJson = null,
            int? order = null,
            bool? isActive = null,
            bool? isPermanent = null,
            DateTime? expiresAt = null)
        {
            if (!string.IsNullOrEmpty(title)) Title = title;
            if (description != null) Description = description;
            if (!string.IsNullOrEmpty(imageUrl)) ImageUrl = imageUrl;
            if (badge != null) Badge = badge;
            if (primaryActionLabel != null) PrimaryActionLabel = primaryActionLabel;
            if (primaryActionLink != null) PrimaryActionLink = primaryActionLink;
            if (secondaryActionLabel != null) SecondaryActionLabel = secondaryActionLabel;
            if (secondaryActionLink != null) SecondaryActionLink = secondaryActionLink;
            if (statsJson != null) StatsJson = statsJson;
            if (order.HasValue) Order = order.Value;
            if (isActive.HasValue) IsActive = isActive.Value;
            
            if (isPermanent.HasValue)
            {
                IsPermanent = isPermanent.Value;
                if (isPermanent.Value)
                {
                    ExpiresAt = null;
                }
                else if (expiresAt.HasValue)
                {
                    ExpiresAt = expiresAt.Value;
                }
                else if (ExpiresAt == null)
                {
                    ExpiresAt = DateTime.UtcNow.AddHours(24);
                }
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return !IsPermanent && ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
        }

        public TimeSpan? GetTimeRemaining()
        {
            if (IsPermanent || !ExpiresAt.HasValue) return null;
            
            var remaining = ExpiresAt.Value - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
}