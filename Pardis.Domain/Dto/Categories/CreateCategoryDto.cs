using Pardis.Domain.Dto.Seo;
using Microsoft.AspNetCore.Http;

namespace Pardis.Domain.Dto.Categories;

public class CreateCategoryDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
    public IFormFile? Image { get; set; }
    public SeoDto? Seo { get; set; }
}

