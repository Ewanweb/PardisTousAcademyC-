using System;
using System.ComponentModel.DataAnnotations;

namespace Pardis.Domain.Sliders
{
    public class SuccessStory : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(100)]
        public string? StudentName { get; set; }

        [MaxLength(200)]
        public string? CourseName { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsPermanent { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }

        // Navigation Properties
        public Guid CreatedByUserId { get; set; }
        public Guid? CourseId { get; set; }

        private SuccessStory() { }

        public static SuccessStory Create(
            string title,
            string imageUrl,
            Guid createdByUserId,
            string? description = null,
            string? studentName = null,
            string? courseName = null,
            string? linkUrl = null,
            Guid? courseId = null,
            int order = 0,
            bool isPermanent = true,
            DateTime? expiresAt = null)
        {
            var story = new SuccessStory
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                ImageUrl = imageUrl,
                StudentName = studentName,
                CourseName = courseName,
                LinkUrl = linkUrl,
                CourseId = courseId,
                Order = order,
                IsActive = true,
                IsPermanent = isPermanent,
                ExpiresAt = isPermanent ? null : expiresAt ?? DateTime.UtcNow.AddHours(24),
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return story;
        }

        public void Update(
            string? title = null,
            string? description = null,
            string? imageUrl = null,
            string? studentName = null,
            string? courseName = null,
            string? linkUrl = null,
            Guid? courseId = null,
            int? order = null,
            bool? isActive = null,
            bool? isPermanent = null,
            DateTime? expiresAt = null)
        {
            if (!string.IsNullOrEmpty(title)) Title = title;
            if (description != null) Description = description;
            if (!string.IsNullOrEmpty(imageUrl)) ImageUrl = imageUrl;
            if (studentName != null) StudentName = studentName;
            if (courseName != null) CourseName = courseName;
            if (linkUrl != null) LinkUrl = linkUrl;
            if (courseId.HasValue) CourseId = courseId.Value;
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