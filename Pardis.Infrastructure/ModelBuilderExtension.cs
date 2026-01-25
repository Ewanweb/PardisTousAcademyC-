using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Accounting;
using Pardis.Domain.Attendance;
using Pardis.Domain.Blog;
using Pardis.Domain.Categories;
using Pardis.Domain.Comments;
using Pardis.Domain.Courses;
using Pardis.Domain.Logging;
using Pardis.Domain.Payments;
using Pardis.Domain.Settings;
using Pardis.Domain.Shopping;
using Pardis.Domain.Users;

namespace Pardis.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder BuildModel(this ModelBuilder builder)
        {
            builder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<Category>().OwnsOne(c => c.Seo);
            builder.Entity<Course>().OwnsOne(c => c.Seo);
            builder.Entity<Course>().OwnsMany(c => c.Sections);

            builder.Entity<UserCourse>().HasKey(uc => new { uc.UserId, uc.CourseId });

            builder.Entity<UserCourse>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.EnrolledCourses)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 تغییر مهم: جلوگیری از حذف آبشاری


            builder.Entity<UserCourse>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(uc => uc.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 تغییر مهم: جلوگیری از حذف آبشاری

            // ✅ تنظیمات CourseSchedule
            builder.Entity<CourseSchedule>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.Schedules)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // حذف زمان‌بندی‌ها با حذف دوره

            // ✅ تنظیمات UserCourseSchedule
            builder.Entity<UserCourseSchedule>()
                .HasKey(ucs => new { ucs.UserId, ucs.CourseScheduleId });

            builder.Entity<UserCourseSchedule>()
                .HasOne(ucs => ucs.User)
                .WithMany()
                .HasForeignKey(ucs => ucs.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserCourseSchedule>()
                .HasOne(ucs => ucs.CourseSchedule)
                .WithMany(cs => cs.StudentEnrollments)
                .HasForeignKey(ucs => ucs.CourseScheduleId)
                .OnDelete(DeleteBehavior.Cascade); // حذف ثبت‌نام‌ها با حذف زمان‌بندی

            // ✅ تنظیمات Transaction
            builder.Entity<Transaction>()
                .HasIndex(t => t.TransactionId)
                .IsUnique();

            builder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.Course)
                .WithMany()
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ تنظیمات CourseComment
            builder.Entity<CourseComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseComment>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseComment>()
                .HasOne(c => c.ReviewedByUser)
                .WithMany()
                .HasForeignKey(c => c.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ تنظیمات CourseSession
            builder.Entity<CourseSession>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseSession>()
                .HasOne(s => s.Schedule)
                .WithMany()
                .HasForeignKey(s => s.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseSession>()
                .HasIndex(s => new { s.CourseId, s.SessionNumber })
                .IsUnique();

            // ✅ تنظیمات StudentAttendance
            builder.Entity<StudentAttendance>()
                .HasOne(a => a.Session)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentAttendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAttendance>()
                .HasOne(a => a.RecordedByUser)
                .WithMany()
                .HasForeignKey(a => a.RecordedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAttendance>()
                .HasIndex(a => new { a.SessionId, a.StudentId })
                .IsUnique();

            // ✅ تنظیمات CourseEnrollment
            builder.Entity<CourseEnrollment>()
                .HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseEnrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseEnrollment>()
                .HasIndex(e => new { e.CourseId, e.StudentId })
                .IsUnique();

            // ✅ تنظیمات InstallmentPayment
            builder.Entity<InstallmentPayment>()
                .HasOne(i => i.Enrollment)
                .WithMany(e => e.InstallmentPayments)
                .HasForeignKey(i => i.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InstallmentPayment>()
                .HasIndex(i => new { i.EnrollmentId, i.InstallmentNumber })
                .IsUnique();

            // ✅ تنظیمات SystemSetting
            builder.Entity<SystemSetting>()
                .HasIndex(s => s.Key)
                .IsUnique();

            // ✅ تنظیمات SystemLog
            builder.Entity<SystemLog>()
                .HasIndex(l => l.Time);

            builder.Entity<SystemLog>()
                .HasIndex(l => new { l.Level, l.Time });

            builder.Entity<SystemLog>()
                .HasIndex(l => new { l.Source, l.Time });

            // ✅ تنظیمات Cart
            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique(); // هر کاربر فقط یک سبد خرید دارد

            // ✅ تنظیمات CartItem
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Course)
                .WithMany()
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.CourseId })
                .IsUnique(); // هر دوره فقط یک بار در سبد

            // ✅ تنظیمات Order
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            // ✅ CartId is required (NOT NULL)
            builder.Entity<Order>()
                .Property(o => o.CartId)
                .IsRequired();

            // ✅ Unique constraint for active orders (re-enabled after migration)
            builder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.CartId })
                .HasFilter("[Status] IN (0, 1)") // Draft=0, PendingPayment=1
                .IsUnique()
                .HasDatabaseName("IX_Orders_UserId_CartId_Active");

            // ✅ تنظیمات PaymentAttempt
            builder.Entity<PaymentAttempt>()
                .HasOne(pa => pa.Order)
                .WithMany(o => o.PaymentAttempts)
                .HasForeignKey(pa => pa.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PaymentAttempt>()
                .HasOne(pa => pa.User)
                .WithMany()
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentAttempt>()
                .HasIndex(pa => pa.TrackingCode)
                .IsUnique();

            builder.Entity<AuthLog>()
                .HasOne(x => x.User)
                .WithMany(x => x.AuthLogs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_AuthLog_User");

            builder.Entity<Post>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Post_User");

            builder.Entity<BlogCategory>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_BlogCategory_User");

            builder.Entity<Post>()
                .HasOne(x => x.BlogCategory)
                .WithMany()
                .HasForeignKey(x => x.BlogCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Post_BlogCategory");


            return builder;
        }
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
                UserName = "09152003530",
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

            modelBuilder.Entity<User>()
                .HasMany(x => x.AuthLogs)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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
                    Location = "https://google.com",
                    Type = CourseType.Online,
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
                    Location = "آموزشگاه",
                    Type = CourseType.InPerson,
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
                    Type = CourseType.Online,
                    Location = "https://google.com",
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

            // --- System Settings ---
            modelBuilder.Entity<SystemSetting>().HasData(
                new
                {
                    Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                    Key = SystemSettingKeys.ManualPaymentCardNumber,
                    Value = "6037-9977-****-****",
                    Description = "شماره کارت مقصد برای پرداخت دستی",
                    IsPublic = true,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                },
                new
                {
                    Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                    Key = SystemSettingKeys.ManualPaymentCardHolder,
                    Value = "آکادمی پردیس توس",
                    Description = "نام صاحب کارت",
                    IsPublic = true,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                },
                new
                {
                    Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                    Key = SystemSettingKeys.ManualPaymentBankName,
                    Value = "بانک پاسارگاد",
                    Description = "نام بانک",
                    IsPublic = true,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                },
                new
                {
                    Id = Guid.Parse("d4444444-4444-4444-4444-444444444444"),
                    Key = SystemSettingKeys.ManualPaymentDescription,
                    Value = "لطفاً پس از واریز، رسید پرداخت را آپلود کنید",
                    Description = "توضیحات پرداخت دستی",
                    IsPublic = true,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                }
            );
        }
    }
}