using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using System;
using System.Text.Json.Serialization;
using Pardis.Domain.Dto.Seo;

namespace Pardis.Application.Categories.Create
{
    public class CreateCategoryCommand : IRequest<OperationResult>
    {
        public string Title { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        [JsonIgnore]
        public string? CurrentUserId { get; set; } // آی‌دی کاربر سازنده
        public SeoDto? Seo { get; set; }
    }
}
