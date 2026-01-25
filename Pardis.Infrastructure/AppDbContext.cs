using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using Pardis.Domain.Accounting;
using Pardis.Domain.Comments;
using Pardis.Domain.Attendance;
using Pardis.Domain.Blog;
using Pardis.Domain.Payments;
using Pardis.Domain.Sliders;
using Pardis.Domain.Settings;
using Pardis.Domain.Shopping;
using Pardis.Domain.Logging;

namespace Pardis.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<AuthLog> AuthLogs { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        
        // ✅ زمان‌بندی دوره‌ها
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<UserCourseSchedule> UserCourseSchedules { get; set; }
        
        // ✅ حسابداری
        public DbSet<Transaction> Transactions { get; set; }
        
        // ✅ کامنت‌ها
        public DbSet<CourseComment> CourseComments { get; set; }
        
        // ✅ حضور و غیاب
        public DbSet<CourseSession> CourseSessions { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        
        // ✅ ثبت‌نام و پرداخت
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<InstallmentPayment> InstallmentPayments { get; set; }
        
        // ✅ تنظیمات سیستم
        public DbSet<PaymentSettings> PaymentSettings { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        
        // ✅ لاگ‌های سیستم
        public DbSet<SystemLog> SystemLogs { get; set; }
        
        // ✅ اسلایدرها و استوری‌ها
        public DbSet<HeroSlide> HeroSlides { get; set; }
        public DbSet<SuccessStory> SuccessStories { get; set; }
        
        // ✅ سبد خرید و سفارش‌ها
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentAttempt> PaymentAttempts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.BuildModel();

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
