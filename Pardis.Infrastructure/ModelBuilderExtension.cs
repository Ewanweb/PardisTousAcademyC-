using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using System;

namespace Pardis.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        // اصلاح: حذف async چون OnModelCreating همگام است
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // تاریخ ثابت برای جلوگیری از خطای PendingModelChanges
            var fixedDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var adminId = Guid.Parse("8e445865-a24d-4543-a6c6-9443d048cdb9");
            var instructorId = Guid.Parse("2c4e6097-f570-4927-b2f7-5f65d1373555");

            var catProgrammingId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
            var catWebId = Guid.Parse("b2222222-2222-2222-2222-222222222222");
            var catMobileId = Guid.Parse("b3333333-3333-3333-3333-333333333333");
            var catArtId = Guid.Parse("b4444444-4444-4444-4444-444444444444");

            var courseAspId = Guid.Parse("c1111111-1111-1111-1111-111111111111");
            var courseReactId = Guid.Parse("c2222222-2222-2222-2222-222222222222");
            var courseFlutterId = Guid.Parse("c3333333-3333-3333-3333-333333333333");

            // --- Users ---
            var hasher = new PasswordHasher<User>();

            var adminUser = new User
            {
                Id = adminId.ToString(),
                UserName = "admin@pardis.com",
                NormalizedUserName = "ADMIN@PARDIS.COM",
                Email = "admin@pardis.com",
                NormalizedEmail = "ADMIN@PARDIS.COM",
                EmailConfirmed = true,
                FullName = "مدیر سیستم",
                IsActive = true,
                SecurityStamp = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                CreatedAt = fixedDate
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "123456");

            var instructorUser = new User
            {
                Id = instructorId.ToString(),
                UserName = "sara@pardis.com",
                NormalizedUserName = "SARA@PARDIS.COM",
                Email = "sara@pardis.com",
                NormalizedEmail = "SARA@PARDIS.COM",
                EmailConfirmed = true,
                FullName = "سارا مدرس",
                IsActive = true,
                SecurityStamp = "2c4e6097-f570-4927-b2f7-5f65d1373555",
                CreatedAt = fixedDate
            };
            instructorUser.PasswordHash = hasher.HashPassword(instructorUser, "123456");

            modelBuilder.Entity<User>().HasData(adminUser, instructorUser);


            // --- Categories ---
            modelBuilder.Entity<Category>().HasData(
                new
                {
                    Id = catProgrammingId,
                    Title = "برنامه نویسی",
                    Slug = "programming",
                    IsActive = true,
                    CoursesCount = 3,
                    ParentId = (Guid?)null,
                    CreatedById = adminId.ToString(),
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = catWebId,
                    Title = "توسعه وب",
                    Slug = "web-development",
                    IsActive = true,
                    CoursesCount = 2,
                    ParentId = (Guid?)catProgrammingId,
                    CreatedById = adminId.ToString(),
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = catMobileId,
                    Title = "برنامه نویسی موبایل",
                    Slug = "mobile-development",
                    IsActive = true,
                    CoursesCount = 1,
                    ParentId = (Guid?)catProgrammingId,
                    CreatedById = adminId.ToString(),
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = catArtId,
                    Title = "هنر و طراحی",
                    Slug = "art-design",
                    IsActive = true,
                    CoursesCount = 0,
                    ParentId = (Guid?)null,
                    CreatedById = instructorId.ToString(),
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate,
                    IsDeleted = false
                }
            );

            // --- Seo Categories ---
            modelBuilder.Entity<Category>().OwnsOne(c => c.Seo).HasData(
                new { CategoryId = catProgrammingId, MetaTitle = "آموزش برنامه نویسی", MetaDescription = "جامع ترین دوره ها", NoIndex = false, NoFollow = false },
                new { CategoryId = catWebId, MetaTitle = "آموزش طراحی سایت", MetaDescription = "ASP.NET و React", NoIndex = false, NoFollow = false },
                new { CategoryId = catMobileId, MetaTitle = "آموزش موبایل", MetaDescription = "فلاتر و کاتلین", NoIndex = false, NoFollow = false },
                new { CategoryId = catArtId, MetaTitle = "آموزش هنر", MetaDescription = "نقاشی و طراحی", NoIndex = false, NoFollow = false }
            );


            // --- Courses ---
            modelBuilder.Entity<Course>().HasData(
                new
                {
                    Id = courseAspId,
                    Title = "دوره جامع ASP.NET Core 8",
                    Slug = "aspnet-core-8-comprehensive",
                    Description = "آموزش پروژه محور",
                    Price = 2500000L,
                    Status = CourseStatus.Published,
                    InstructorId = adminId.ToString(),
                    CategoryId = catWebId,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate,
                    Schedule = "Saturday 10:00",
                    StartFrom = "Saturday 10:00",
                    IsCompleted = true,
                    IsStarted = true,
                    IsDeleted = false
                },
                new
                {
                    Id = courseReactId,
                    Title = "متخصص React و Next.js",
                    Slug = "react-nextjs-mastery",
                    Description = "از مقدماتی تا پیشرفته",
                    Price = 1800000L,
                    Status = CourseStatus.Published,
                    InstructorId = instructorId.ToString(),
                    CategoryId = catWebId,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate,
                    Schedule = "Saturday 10:00",
                    StartFrom = "Saturday 10:00",
                    IsCompleted = false,
                    IsStarted = true,
                    IsDeleted = false
                },
                new
                {
                    Id = courseFlutterId,
                    Title = "فلاتر: از صفر تا صد",
                    Slug = "flutter-zero-to-hero",
                    Description = "ساخت اپلیکیشن",
                    Price = 3000000L,
                    Status = CourseStatus.Draft,
                    InstructorId = adminId.ToString(),
                    CategoryId = catMobileId,
                    CreatedAt = fixedDate,
                    Schedule = "Saturday 10:00",
                    StartFrom = "Saturday 10:00",
                    IsCompleted = false,
                    IsStarted = false,
                    UpdatedAt = fixedDate,
                    IsDeleted = false
                }
            );

            // --- Seo Courses ---
            modelBuilder.Entity<Course>().OwnsOne(c => c.Seo).HasData(
                new { CourseId = courseAspId, MetaTitle = "دوره ASP.NET Core", MetaDescription = "بهترین دوره بک اند", NoIndex = false, NoFollow = false },
                new { CourseId = courseReactId, MetaTitle = "دوره React", MetaDescription = "آموزش فرانت اند", NoIndex = false, NoFollow = false },
                new { CourseId = courseFlutterId, MetaTitle = "دوره Flutter", MetaDescription = "برنامه نویسی موبایل", NoIndex = true, NoFollow = false }
            );
        }
    }
}