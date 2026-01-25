using Pardis.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Pardis.Domain.Dto.Blog
{
    public class PostDTO
    {
        [JsonIgnore]
        public string? UserId { get; set; }
        public Guid BlogCategoryId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SummaryDescription { get; set; } = string.Empty;
        public IFormFile Thumbnail { get; set; }
        public SeoDto SeoMetadata { get; set; } = new SeoDto();
        public long Views { get; set; }
    }
}
