using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pardis.Domain.Dto.Sliders
{
    // Request DTOs
    public class CreateSuccessStoryDto
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(200, ErrorMessage = "عنوان نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "توضیحات نمی‌تواند بیش از 1000 کاراکتر باشد")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "نام دانشجو نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? StudentName { get; set; }

        [MaxLength(200, ErrorMessage = "نام دوره نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string? CourseName { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس لینک نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? LinkUrl { get; set; }

        public Guid? CourseId { get; set; }

        public int Order { get; set; } = 0;

        public bool IsPermanent { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }
    }

    public class UpdateSuccessStoryDto
    {
        [MaxLength(200, ErrorMessage = "عنوان نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string? Title { get; set; }

        [MaxLength(1000, ErrorMessage = "توضیحات نمی‌تواند بیش از 1000 کاراکتر باشد")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "نام دانشجو نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string? StudentName { get; set; }

        [MaxLength(200, ErrorMessage = "نام دوره نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string? CourseName { get; set; }

        [MaxLength(500, ErrorMessage = "آدرس لینک نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string? LinkUrl { get; set; }

        public Guid? CourseId { get; set; }

        public int? Order { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsPermanent { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    // Response DTOs
    public class SuccessStoryResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
        public string? LinkUrl { get; set; }
        public Guid? CourseId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
    }

    public class SuccessStoryListResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
        public string? LinkUrl { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public bool IsExpired { get; set; }
    }
}