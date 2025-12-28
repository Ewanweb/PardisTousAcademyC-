using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pardis.Domain.Dto.Sliders
{
    // Request DTOs
    public class CreateSuccessStoryDto
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(200, ErrorMessage = "عنوان نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "برچسب اکشن نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? ActionLabel { get; set; }

        [MaxLength(500, ErrorMessage = "لینک اکشن نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ActionLink { get; set; }

        public int Order { get; set; } = 0;
    }

    public class UpdateSuccessStoryDto
    {
        [MaxLength(200, ErrorMessage = "عنوان نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "برچسب اکشن نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? ActionLabel { get; set; }

        [MaxLength(500, ErrorMessage = "لینک اکشن نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ActionLink { get; set; }

        public int? Order { get; set; }

        public bool? IsActive { get; set; }
    }

    // Response DTOs
    public class SuccessStoryResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? ActionLabel { get; set; }
        public string? ActionLink { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
    }

    public class SuccessStoryListResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? ActionLabel { get; set; }
        public string? ActionLink { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}