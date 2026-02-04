using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Seo;

namespace Pardis.Application.Seo.Resolvers
{
    public class CourseSeoResolver : ISeoResolver<Course>, ISeoResolverBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly string _brandName = "آکادمی پردیس توس";

        public SeoEntityType EntityType => SeoEntityType.Course;

        public CourseSeoResolver(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<SeoDto> ResolveAsync(Course entity, SeoContext context)
        {
            var seoMeta = entity.Seo;
            var canonicalUrl = BuildCanonicalUrl(entity, context);
            var jsonLdSchemas = GenerateJsonLdSchemas(entity, context);
            var breadcrumbs = GenerateBreadcrumbs(entity, context);
            var robotsContent = GetRobotsContent(entity, context);

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
                OgImage = seoMeta?.OpenGraphImage ?? entity.Thumbnail ?? GetDefaultImage(),
                OgType = seoMeta?.OpenGraphType ?? "article",
                OgSiteName = _brandName,
                OgLocale = context.Language == "en" ? "en_US" : "fa_IR",
                
                TwitterTitle = seoMeta?.TwitterTitle ?? title,
                TwitterDescription = seoMeta?.TwitterDescription ?? description,
                TwitterImage = seoMeta?.TwitterImage ?? seoMeta?.OpenGraphImage ?? entity.Thumbnail ?? GetDefaultImage(),
                TwitterCard = seoMeta?.TwitterCardType ?? "summary_large_image",
                
                JsonLdSchemas = jsonLdSchemas,
                Breadcrumbs = breadcrumbs,
                
                Author = entity.Instructor?.FullName,
                LastModified = entity.UpdatedAt,
                Language = context.Language,
                Direction = context.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = canonicalUrl
            };
        }

        public async Task<SeoDto> ResolveBySlugAsync(string slug, SeoContext context)
        {
            var course = await _courseRepository.Table
                .Include(c => c.Seo)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Slug == slug);
            if (course == null)
                throw new InvalidOperationException($"Course with slug '{slug}' not found");

            return await ResolveAsync(course, context);
        }

        public SeoDto GenerateDefaultSeo(Course entity, SeoContext context)
        {
            return new SeoDto
            {
                MetaTitle = GenerateTitle(entity, context),
                MetaDescription = GenerateDescription(entity, context),
                Keywords = GenerateKeywords(entity),
                CanonicalUrl = BuildCanonicalUrl(entity, context),
                RobotsContent = GetRobotsContent(entity, context),
                JsonLdSchemas = GenerateJsonLdSchemas(entity, context),
                Breadcrumbs = GenerateBreadcrumbs(entity, context),
                Language = context.Language,
                Direction = context.Language == "en" ? "ltr" : "rtl",
                CurrentUrl = BuildCanonicalUrl(entity, context)
            };
        }

        public string BuildCanonicalUrl(Course entity, SeoContext context)
        {
            var baseUrl = context.BaseUrl.TrimEnd('/');
            // Always use the main course URL, regardless of preview/purchased state
            return $"{baseUrl}/course/{entity.Slug}";
        }

        public List<object> GenerateJsonLdSchemas(Course entity, SeoContext context)
        {
            var schemas = new List<object>();

            // Course Schema
            var courseSchema = new
            {
                context = "https://schema.org",
                type = "Course",
                name = entity.Title,
                description = entity.Description ?? GenerateDescription(entity, context),
                provider = new
                {
                    type = "Organization",
                    name = _brandName,
                    url = context.BaseUrl
                },
                url = BuildCanonicalUrl(entity, context),
                courseMode = "online",
                inLanguage = context.Language == "en" ? "en" : "fa",
                image = entity.Thumbnail,
                offers = entity.Price > 0 ? new
                {
                    type = "Offer",
                    price = entity.Price,
                    priceCurrency = "IRR",
                    availability = "https://schema.org/InStock",
                    validFrom = entity.CreatedAt.ToString("yyyy-MM-dd")
                } : null,
                instructor = entity.Instructor != null ? new
                {
                    type = "Person",
                    name = entity.Instructor.FullName,
                    description = entity.Instructor.Bio
                } : null,
                hasCourseInstance = new
                {
                    type = "CourseInstance",
                    courseMode = "online",
                    instructor = entity.Instructor != null ? new
                    {
                        type = "Person",
                        name = entity.Instructor.FullName
                    } : null
                }
            };

            schemas.Add(courseSchema);

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

        public List<BreadcrumbItem> GenerateBreadcrumbs(Course entity, SeoContext context)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new() { 
                    Name = context.Language == "en" ? "Home" : "خانه", 
                    Url = "/", 
                    Position = 1 
                }
            };

            if (entity.Category != null)
            {
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = entity.Category.Title,
                    Url = $"/category/{entity.Category.Slug}",
                    Position = 2
                });
                
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = entity.Title,
                    Url = $"/course/{entity.Slug}",
                    Position = 3
                });
            }
            else
            {
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = entity.Title,
                    Url = $"/course/{entity.Slug}",
                    Position = 2
                });
            }

            return breadcrumbs;
        }

        private string GenerateTitle(Course entity, SeoContext context)
        {
            var title = entity.Title;
            

            var suffix = context.Language == "en" ? "Course" : "دوره آموزشی";
            return $"{title} | {suffix} | {_brandName}";
        }

        private string GenerateDescription(Course entity, SeoContext context)
        {
            if (!string.IsNullOrEmpty(entity.Description))
            {
                var description = entity.Description.Length > 140 
                    ? entity.Description.Substring(0, 137) + "..."
                    : entity.Description;

                // Add course details
                var details = new List<string>();
                
                if (entity.Price > 0)
                {
                    var priceText = context.Language == "en"
                        ? $"{entity.Price:N0} IRR"
                        : $"{entity.Price:N0} تومان";
                    details.Add(priceText);
                }

                if (details.Any())
                {
                    var detailsText = string.Join(" | ", details);
                    var maxDescLength = 160 - detailsText.Length - 3; // 3 for " - "
                    
                    if (description.Length > maxDescLength)
                    {
                        description = description.Substring(0, maxDescLength - 3) + "...";
                    }
                    
                    return $"{description} - {detailsText}";
                }

                return description;
            }

            var fallbackText = context.Language == "en"
                ? $"{entity.Title} course with real projects and mentor support at {_brandName}"
                : $"دوره آموزشی {entity.Title} با پروژه‌های واقعی و پشتیبانی منتور در {_brandName}";
            
            return fallbackText;
        }

        private string GenerateKeywords(Course entity)
        {
            var keywords = new List<string>
            {
                entity.Title,
                "دوره آموزشی",
                "آموزش آنلاین"
            };

            if (entity.Category != null)
            {
                keywords.Add(entity.Category.Title);
            }


            if (entity.Instructor != null)
            {
                keywords.Add(entity.Instructor.FullName);
            }

            keywords.Add(_brandName);

            return string.Join(", ", keywords.Take(10));
        }

        private string GetRobotsContent(Course entity, SeoContext context)
        {
            // Don't index unpublished courses
            if (entity.Status != CourseStatus.Published)
                return "noindex, nofollow";

            // Don't index preview pages for bots (but allow for users)
            if (context.IsPreview && IsBot(context.UserAgent))
                return "noindex, follow";

            return "index, follow";
        }

        private bool IsBot(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return false;

            var botPatterns = new[] { "googlebot", "bingbot", "slurp", "duckduckbot", "baiduspider", "yandexbot" };
            return botPatterns.Any(pattern => userAgent.ToLower().Contains(pattern));
        }

        private string GetDefaultImage()
        {
            return "/images/course-default.jpg";
        }

    }
}
