using System;
using System.Collections.Generic;
namespace Pardis.Domain.Dto.Seo;

/// <summary>
/// SEO data transfer object
/// </summary>
public class SeoDto
{
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    
    // OpenGraph
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImage { get; set; }
    public string? OgType { get; set; } = "website";
    public string? OgSiteName { get; set; }
    public string? OgLocale { get; set; } = "fa_IR";
    
    // Twitter Card
    public string? TwitterTitle { get; set; }
    public string? TwitterDescription { get; set; }
    public string? TwitterImage { get; set; }
    public string? TwitterCard { get; set; } = "summary_large_image";
    
    // Additional SEO properties
    public string? Author { get; set; }
    public string? RobotsContent { get; set; }
    public string? Direction { get; set; } = "rtl";
    public string? Language { get; set; }
    public string? CurrentUrl { get; set; }
    public string? PrevUrl { get; set; }
    public string? NextUrl { get; set; }
    public DateTime? LastModified { get; set; }
    
    // JSON-LD schemas
    public List<object>? JsonLdSchemas { get; set; }

    // Breadcrumbs
    public List<BreadcrumbItem>? Breadcrumbs { get; set; }
}

public class BreadcrumbItem
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Position { get; set; }
}

