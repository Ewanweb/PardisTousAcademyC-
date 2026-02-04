using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Categories;
using Pardis.Domain.Seo;
using Pardis.Domain.Dto.Seo;
using Pardis.Infrastructure.Repository;

namespace Pardis.Application.Seo.Resolvers
{
    public class CategorySeoResolver : ISeoResolver<Category>, ISeoResolverBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly string _brandName = "آکادمی پردیس توس";
        private readonly HashSet<string> _allowedParams = new() { "page", "level", "price", "sort" };

        public SeoEntityType EntityType => SeoEntityType.Category;

        public CategorySeoResolver(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<SeoDto> ResolveAsync(Category entity, SeoContext context)
        {
            var seoMeta = entity.Seo;
            var canonicalUrl = BuildCanonicalUrl(entity, context);
            var jsonLdSchemas = GenerateJsonLdSchemas(entity, context);
            var breadcrumbs = GenerateBreadcrumbs(entity, context);
            var robotsContent = GetRobotsContent(context);

            var title = seoMeta?.MetaTitle ?? GenerateTitle(entity, context);
            var description = seoMeta?.MetaDescription ?? GenerateDescription(entity, context);

            return new SeoDto
            {
                MetaTitle = title,
                MetaDescription = description,
                Keywords = seoMeta?.Keywords ?? GenerateKeywords(entity),
                CanonicalUrl = canonicalUrl,
                RobotsContent = robotsContent,
                
                OgTitle = seoMeta?.OpenGraphTitle ?? title,
                OgDescription = seoMeta?.OpenGraphDescription ?? description,
                OgImage = seoMeta?.OpenGraphImage ?? GetDefaultImage(entity),
                OgType = seoMeta?.OpenGraphType ?? "website",
                OgSiteName = _brandName,
                OgLocale = context.Language == "en" ? "en_US" : "fa_IR",
                
                TwitterTitle = seoMeta?.TwitterTitle ?? title,
                TwitterDescription = seoMeta?.TwitterDescription ?? description,
                TwitterImage = seoMeta?.TwitterImage ?? seoMeta?.OpenGraphImage ?? GetDefaultImage(entity),
                TwitterCard = seoMeta?.TwitterCardType ?? "summary_large_image",
                
                JsonLdSchemas = jsonLdSchemas,
                Breadcrumbs = breadcrumbs,
                
                LastModified = entity.UpdatedAt,
                PrevUrl = GeneratePrevUrl(context),
                NextUrl = GenerateNextUrl(context),
                Language = context.Language,
                Direction = context.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = canonicalUrl
            };
        }

        public async Task<SeoDto> ResolveBySlugAsync(string slug, SeoContext context)
        {
            var category = await _categoryRepository.Table
                .Include(c => c.Seo)
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Slug == slug);
            if (category == null)
                throw new InvalidOperationException($"Category with slug '{slug}' not found");

            return await ResolveAsync(category, context);
        }

        public SeoDto GenerateDefaultSeo(Category entity, SeoContext context)
        {
            return new SeoDto
            {
                MetaTitle = GenerateTitle(entity, context),
                MetaDescription = GenerateDescription(entity, context),
                Keywords = GenerateKeywords(entity),
                CanonicalUrl = BuildCanonicalUrl(entity, context),
                RobotsContent = GetRobotsContent(context),
                JsonLdSchemas = GenerateJsonLdSchemas(entity, context),
                Breadcrumbs = GenerateBreadcrumbs(entity, context),
                Language = context.Language,
                Direction = context.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = BuildCanonicalUrl(entity, context)
            };
        }

        public string BuildCanonicalUrl(Category entity, SeoContext context)
        {
            var baseUrl = context.BaseUrl.TrimEnd('/');
            var path = $"/category/{entity.Slug}";

            // Handle pagination - page=1 should not have page parameter
            if (context.Page.HasValue && context.Page.Value > 1)
            {
                var filteredParams = context.QueryParams
                    .Where(p => _allowedParams.Contains(p.Key.ToLower()))
                    .ToDictionary(p => p.Key, p => p.Value);
                
                filteredParams["page"] = context.Page.Value.ToString();
                
                var queryString = string.Join("&", 
                    filteredParams.OrderBy(p => p.Key)
                    .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                
                return $"{baseUrl}{path}?{queryString}";
            }

            // Handle other allowed parameters (excluding page=1)
            var allowedQueryParams = context.QueryParams
                .Where(p => _allowedParams.Contains(p.Key.ToLower()) && p.Key.ToLower() != "page")
                .OrderBy(p => p.Key)
                .ToList();

            if (allowedQueryParams.Any())
            {
                var queryString = string.Join("&", 
                    allowedQueryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                return $"{baseUrl}{path}?{queryString}";
            }

            return $"{baseUrl}{path}";
        }

        public List<object> GenerateJsonLdSchemas(Category entity, SeoContext context)
        {
            var schemas = new List<object>();

            // CollectionPage Schema
            schemas.Add(new
            {
                context = "https://schema.org",
                type = "CollectionPage",
                name = entity.Title,
                description = entity.Description ?? GenerateDescription(entity, context),
                url = BuildCanonicalUrl(entity, context),
                mainEntity = new
                {
                    type = "ItemList",
                    name = context.Language == "en" ? $"{entity.Title} Courses" : $"دوره‌های {entity.Title}",
                    numberOfItems = entity.GetSeoContext().TryGetValue("courseCount", out var count) ? count : 0
                }
            });

            // Organization Schema
            schemas.Add(new
            {
                context = "https://schema.org",
                type = "EducationalOrganization",
                name = _brandName,
                url = context.BaseUrl,
                sameAs = new[]
                {
                    "https://instagram.com/pardistous",
                    "https://telegram.me/pardistous"
                }
            });

            return schemas;
        }

        public List<BreadcrumbItem> GenerateBreadcrumbs(Category entity, SeoContext context)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new() { 
                    Name = context.Language == "en" ? "Home" : "خانه", 
                    Url = "/", 
                    Position = 1 
                }
            };

            // Add parent category if exists
            if (entity.ParentId.HasValue && entity.Parent != null)
            {
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = entity.Parent.Title,
                    Url = $"/category/{entity.Parent.Slug}",
                    Position = 2
                });
                
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = entity.Title,
                    Url = $"/category/{entity.Slug}",
                    Position = 3
                });
            }
            else
            {
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = entity.Title,
                    Url = $"/category/{entity.Slug}",
                    Position = 2
                });
            }

            return breadcrumbs;
        }

        private string GenerateTitle(Category entity, SeoContext context)
        {
            var title = entity.Title;
            
            if (context.Page.HasValue && context.Page.Value > 1)
            {
                var pageText = context.Language == "en" ? $"Page {context.Page.Value}" : $"صفحه {context.Page.Value}";
                title = $"{title} - {pageText}";
            }

            var suffix = context.Language == "en" ? "Courses" : "دوره‌های";
            return $"{suffix} {title} | {_brandName}";
        }

        private string GenerateDescription(Category entity, SeoContext context)
        {
            if (!string.IsNullOrEmpty(entity.Description))
            {
                return entity.Description.Length > 160 
                    ? entity.Description.Substring(0, 157) + "..."
                    : entity.Description;
            }

            var courseCount = entity.GetSeoContext().TryGetValue("courseCount", out var count) ? count.ToString() : "";
            var courseText = !string.IsNullOrEmpty(courseCount) && int.Parse(courseCount) > 0 
                ? (context.Language == "en" ? $"with {courseCount} courses " : $"با {courseCount} دوره ")
                : "";
            
            return context.Language == "en"
                ? $"Specialized {entity.Title} courses {courseText}from beginner to advanced. Step-by-step learning with real projects and mentor support."
                : $"دوره‌های تخصصی {entity.Title} {courseText}از مبتدی تا پیشرفته. آموزش قدم‌به‌قدم با پروژه‌های واقعی و پشتیبانی منتور.";
        }

        private string GenerateKeywords(Category entity)
        {
            var keywords = new List<string>
            {
                entity.Title,
                "دوره آموزشی",
                "آموزش آنلاین",
                _brandName
            };

            if (entity.Parent != null)
            {
                keywords.Add(entity.Parent.Title);
            }

            return string.Join(", ", keywords.Take(10));
        }

        private string GetRobotsContent(SeoContext context)
        {
            // Don't index deep pagination
            if (context.Page.HasValue && context.Page.Value > 10)
                return "noindex, follow";

            // Don't index complex filter combinations
            var filterCount = context.QueryParams.Count(p => p.Key.ToLower() != "page");
            if (filterCount > 2)
                return "noindex, follow";

            return "index, follow";
        }

        private string? GetDefaultImage(Category entity)
        {
            return entity.Image ?? "/images/category-default.jpg";
        }

        private string? GeneratePrevUrl(SeoContext context)
        {
            if (context.Page.HasValue && context.Page.Value > 1)
            {
                var prevPage = context.Page.Value - 1;
                var basePath = context.CurrentPath?.Split('?')[0] ?? "";
                
                if (prevPage == 1)
                {
                    // Remove page parameter for page 1
                    var otherParams = context.QueryParams
                        .Where(p => p.Key.ToLower() != "page")
                        .OrderBy(p => p.Key)
                        .ToList();
                    
                    if (otherParams.Any())
                    {
                        var queryString = string.Join("&", 
                            otherParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                        return $"{basePath}?{queryString}";
                    }
                    return basePath;
                }
                else
                {
                    var allParams = context.QueryParams.ToDictionary(p => p.Key, p => p.Value);
                    allParams["page"] = prevPage.ToString();
                    var queryString = string.Join("&", 
                        allParams.OrderBy(p => p.Key)
                        .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                    return $"{basePath}?{queryString}";
                }
            }
            return null;
        }

        private string? GenerateNextUrl(SeoContext context)
        {
            // This would require knowing if there are more pages
            // For now, return null - would be implemented based on total count
            return null;
        }
    }
}
