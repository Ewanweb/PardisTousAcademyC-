using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Categories.Create
{
    public class CreateCategoryCommand : IRequest<OperationResult>
    {
        public string Title { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public string? CurrentUserId { get; set; } // آی‌دی کاربر سازنده
        public SeoDto Seo { get; set; }
    }
}
