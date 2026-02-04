using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Application.Seo;
using Pardis.Application.Seo.Resolvers;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Pages;
using Pardis.Domain.Seo;

namespace Pardis.Infrastructure
{
    public static class SeoConfiguration
    {
        public static IServiceCollection AddSeoServices(this IServiceCollection services)
        {
            // Application SEO services
            services.AddScoped<Pardis.Application.Seo.ISeoService, Pardis.Application.Seo.SeoService>();
            services.AddScoped<Pardis.Domain.Seo.ISeoService, Pardis.Domain.Seo.SeoService>();
            services.AddScoped<ISlugRedirectService, SlugRedirectService>();
            services.AddScoped<ISeoSafetyService, SeoSafetyService>();

            // SEO resolvers
            services.AddScoped<CategorySeoResolver>();
            services.AddScoped<CourseSeoResolver>();
            services.AddScoped<PageSeoResolver>();

            services.AddMemoryCache();

            return services;
        }

        public static void ConfigureSeoEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().OwnsOne(c => c.Seo, seo =>
            {
                seo.Property(s => s.MetaTitle).HasMaxLength(60);
                seo.Property(s => s.MetaDescription).HasMaxLength(160);
                seo.Property(s => s.Keywords).HasMaxLength(255);
                seo.Property(s => s.CanonicalUrl).HasMaxLength(500);
                seo.Property(s => s.OpenGraphTitle).HasMaxLength(60);
                seo.Property(s => s.OpenGraphDescription).HasMaxLength(160);
                seo.Property(s => s.OpenGraphImage).HasMaxLength(500);
                seo.Property(s => s.OpenGraphType).HasMaxLength(20).HasDefaultValue("website");
                seo.Property(s => s.TwitterTitle).HasMaxLength(60);
                seo.Property(s => s.TwitterDescription).HasMaxLength(160);
                seo.Property(s => s.TwitterImage).HasMaxLength(500);
                seo.Property(s => s.TwitterCardType).HasMaxLength(20).HasDefaultValue("summary_large_image");
                seo.Property(s => s.JsonLdSchemas);
            });

            modelBuilder.Entity<Course>().OwnsOne(c => c.Seo, seo =>
            {
                seo.Property(s => s.MetaTitle).HasMaxLength(60);
                seo.Property(s => s.MetaDescription).HasMaxLength(160);
                seo.Property(s => s.Keywords).HasMaxLength(255);
                seo.Property(s => s.CanonicalUrl).HasMaxLength(500);
                seo.Property(s => s.OpenGraphTitle).HasMaxLength(60);
                seo.Property(s => s.OpenGraphDescription).HasMaxLength(160);
                seo.Property(s => s.OpenGraphImage).HasMaxLength(500);
                seo.Property(s => s.OpenGraphType).HasMaxLength(20).HasDefaultValue("website");
                seo.Property(s => s.TwitterTitle).HasMaxLength(60);
                seo.Property(s => s.TwitterDescription).HasMaxLength(160);
                seo.Property(s => s.TwitterImage).HasMaxLength(500);
                seo.Property(s => s.TwitterCardType).HasMaxLength(20).HasDefaultValue("summary_large_image");
                seo.Property(s => s.JsonLdSchemas);
            });

            modelBuilder.Entity<Page>().OwnsOne(p => p.Seo, seo =>
            {
                seo.Property(s => s.MetaTitle).HasMaxLength(60);
                seo.Property(s => s.MetaDescription).HasMaxLength(160);
                seo.Property(s => s.Keywords).HasMaxLength(255);
                seo.Property(s => s.CanonicalUrl).HasMaxLength(500);
                seo.Property(s => s.OpenGraphTitle).HasMaxLength(60);
                seo.Property(s => s.OpenGraphDescription).HasMaxLength(160);
                seo.Property(s => s.OpenGraphImage).HasMaxLength(500);
                seo.Property(s => s.OpenGraphType).HasMaxLength(20).HasDefaultValue("website");
                seo.Property(s => s.TwitterTitle).HasMaxLength(60);
                seo.Property(s => s.TwitterDescription).HasMaxLength(160);
                seo.Property(s => s.TwitterImage).HasMaxLength(500);
                seo.Property(s => s.TwitterCardType).HasMaxLength(20).HasDefaultValue("summary_large_image");
                seo.Property(s => s.JsonLdSchemas);
            });

            modelBuilder.Entity<SlugRedirect>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OldSlug).HasMaxLength(255).IsRequired();
                entity.Property(e => e.NewSlug).HasMaxLength(255).IsRequired();
                entity.Property(e => e.EntityType).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => new { e.OldSlug, e.EntityType });
                entity.HasIndex(e => e.IsActive);
            });

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.HasIndex(p => p.PageType);
                entity.HasIndex(p => p.IsPublished);
                entity.HasIndex(p => p.Language);
            });
        }
    }
}
