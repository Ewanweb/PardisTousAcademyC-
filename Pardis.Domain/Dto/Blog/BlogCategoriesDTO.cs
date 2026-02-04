using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Pardis.Domain.Dto.Blog
{
    public class BlogCategoriesDTO
    {
        [JsonIgnore]
        public string UserId { get;  set; } = string.Empty;
        public string CreatedBy { get;  set; } = string.Empty;
        public string Title { get;  set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public IFormFile Thumbnail { get;  set; } 
        public SeoDto SeoMetadata { get; set; } = new SeoDto();
    }
}


