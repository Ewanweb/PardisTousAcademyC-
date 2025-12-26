using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pardis.Domain.Dto.Sliders
{
    // Request DTOs
    public class CreateHeroSlideDto
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(200, ErrorMessage = "عنوان نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "نشان نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? Badge { get; set; }

        // Primary Action
        [MaxLength(100, ErrorMessage = "برچسب اکشن اصلی نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? PrimaryActionLabel { get; set; }

        [MaxLength(500, ErrorMessage = "لینک اکشن اصلی نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? PrimaryActionLink { get; set; }

        // Secondary Action
        [MaxLength(100, ErrorMessage = "برچسب اکشن ثانویه نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? SecondaryActionLabel { get; set; }

        [MaxLength(500, ErrorMessage = "لینک اکشن ثانویه نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? SecondaryActionLink { get; set; }

        // Stats
        public List<SlideStatDto>? Stats { get; set; }

        public int Order { get; set; } = 0;

        public bool IsPermanent { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }

        // Legacy properties for backward compatibility
        [MaxLength(500, ErrorMessage = "آدرس لینک نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? LinkUrl { get; set; }

        [MaxLength(100, ErrorMessage = "متن دکمه نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? ButtonText { get; set; }
    }

    public class UpdateHeroSlideDto
    {
        [MaxLength(200, ErrorMessage = "عنوان نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "نشان نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? Badge { get; set; }

        // Primary Action
        [MaxLength(100, ErrorMessage = "برچسب اکشن اصلی نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? PrimaryActionLabel { get; set; }

        [MaxLength(500, ErrorMessage = "لینک اکشن اصلی نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? PrimaryActionLink { get; set; }

        // Secondary Action
        [MaxLength(100, ErrorMessage = "برچسب اکشن ثانویه نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? SecondaryActionLabel { get; set; }

        [MaxLength(500, ErrorMessage = "لینک اکشن ثانویه نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? SecondaryActionLink { get; set; }

        // Stats
        public List<SlideStatDto>? Stats { get; set; }

        public int? Order { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsPermanent { get; set; }

        public DateTime? ExpiresAt { get; set; }

        // Legacy properties for backward compatibility
        [MaxLength(500, ErrorMessage = "آدرس لینک نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? LinkUrl { get; set; }

        [MaxLength(100, ErrorMessage = "متن دکمه نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? ButtonText { get; set; }
    }

    // Response DTOs
    public class HeroSlideResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Badge { get; set; }
        public SlideActionDto? PrimaryAction { get; set; }
        public SlideActionDto? SecondaryAction { get; set; }
        public List<SlideStatDto>? Stats { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }

        // Legacy properties for backward compatibility
        public string? LinkUrl { get; set; }
        public string? ButtonText { get; set; }
    }

    public class HeroSlideListResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Badge { get; set; }
        public SlideActionDto? PrimaryAction { get; set; }
        public SlideActionDto? SecondaryAction { get; set; }
        public List<SlideStatDto>? Stats { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }

        // Legacy properties for backward compatibility
        public string? LinkUrl { get; set; }
        public string? ButtonText { get; set; }
    }
}