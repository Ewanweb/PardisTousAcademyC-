using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        
        // ✅ زمان‌بندی دوره‌ها
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<UserCourseSchedule> UserCourseSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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

            builder.Seed();
        }

        // --- این متد را اضافه کنید تا خطا رفع شود ---
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // نادیده گرفتن خطای تغییرات مدل (Pending Model Changes)
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}
