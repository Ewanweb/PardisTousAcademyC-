using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pardis.Application._Shared.ExtensionMappers
{
    public static class Mapper
    {
        public static Post ToEntity(this PostDTO dto)
        {
            return new Post()
            {
                Title = dto.Title,
                BlogCategoryId = dto.BlogCategoryId,
                Description = dto.Description,
                SummaryDescription = dto.SummaryDescription,
                Slug = dto.Slug,
                SeoMetadata =
                {
                    CanonicalUrl = dto.SeoMetadata.CanonicalUrl,
                    MetaDescription = dto.SeoMetadata.MetaDescription,
                    MetaTitle = dto.SeoMetadata.MetaTitle,
                    NoFollow = dto.SeoMetadata.NoFollow,
                    NoIndex = dto.SeoMetadata.NoIndex
                }
            };
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
                SeoMetadata =
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

        public static BlogCategory ToEntity(this BlogCategoriesDTO dto)
        {
            return new BlogCategory()
            {
                Title = dto.Title,
                Slug = dto.Slug,
                SeoMetadata =
                {
                    CanonicalUrl = dto.SeoMetadata.CanonicalUrl,
                    MetaDescription = dto.SeoMetadata.MetaDescription,
                    MetaTitle = dto.SeoMetadata.MetaTitle,
                    NoFollow = dto.SeoMetadata.NoFollow,
                    NoIndex = dto.SeoMetadata.NoIndex
                },

            };
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
                SeoMetadata =
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
