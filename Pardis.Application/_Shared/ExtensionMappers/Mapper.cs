using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Seo;
using Pardis.Domain.Seo;

namespace Pardis.Application._Shared.ExtensionMappers
{
    public static class Mapper
    {
        public static Post ToEntity(this PostDTO dto)
        {
            var post = new Post()
            {
                Title = dto.Title,
                BlogCategoryId = dto.BlogCategoryId,
                Description = dto.Description,
                SummaryDescription = dto.SummaryDescription,
                Slug = dto.Slug
            };

            if (dto.SeoMetadata != null)
            {
                var seoMetadata = new SeoMetadata(
                    metaTitle: dto.SeoMetadata.MetaTitle,
                    metaDescription: dto.SeoMetadata.MetaDescription,
                    canonicalUrl: dto.SeoMetadata.CanonicalUrl,
                    noIndex: dto.SeoMetadata.NoIndex,
                    noFollow: dto.SeoMetadata.NoFollow
                );
                post.SeoMetadata = seoMetadata;
            }

            return post;
        }

        public static PostResource ToResource(this Post entity)
        {
            return new PostResource()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                Author = entity.Author,
                BlogCategoryId = entity.BlogCategoryId,
                Description = entity.Description,
                SummaryDescription = entity.SummaryDescription,
                ThumbnailUrl = entity.ThumbnailUrl,
                Slug = entity.Slug,
                SeoMetadata = entity.SeoMetadata != null ? new SeoDto
                {
                    CanonicalUrl = entity.SeoMetadata.CanonicalUrl,
                    MetaDescription = entity.SeoMetadata.MetaDescription,
                    MetaTitle = entity.SeoMetadata.MetaTitle,
                    NoFollow = entity.SeoMetadata.NoFollow,
                    NoIndex = entity.SeoMetadata.NoIndex
                } : new SeoDto(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static BlogCategory ToEntity(this BlogCategoriesDTO dto)
        {
            var blogCategory = new BlogCategory()
            {
                Title = dto.Title,
                Slug = dto.Slug
            };

            if (dto.SeoMetadata != null)
            {
                var seoMetadata = new SeoMetadata(
                    metaTitle: dto.SeoMetadata.MetaTitle,
                    metaDescription: dto.SeoMetadata.MetaDescription,
                    canonicalUrl: dto.SeoMetadata.CanonicalUrl,
                    noIndex: dto.SeoMetadata.NoIndex,
                    noFollow: dto.SeoMetadata.NoFollow
                );
                blogCategory.SeoMetadata = seoMetadata;
            }

            return blogCategory;
        }

        public static BlogCategoriesResource ToResource(this BlogCategory entity)
        {
            return new BlogCategoriesResource()
            {
                Id = entity.Id,
                Title = entity.Title,
                CreatedBy = entity.CreatedBy,
                UserId = entity.UserId,
                Thumbnail = entity.Thumbnail,
                ThumbnailUrl = entity.ThumbnailUrl,
                Slug = entity.Slug,
                SeoMetadata = new SeoDto
                {
                    CanonicalUrl = entity.SeoMetadata.CanonicalUrl,
                    MetaDescription = entity.SeoMetadata.MetaDescription,
                    MetaTitle = entity.SeoMetadata.MetaTitle,
                    NoFollow = entity.SeoMetadata.NoFollow,
                    NoIndex = entity.SeoMetadata.NoIndex
                },
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,

            };
        }
    }
}
