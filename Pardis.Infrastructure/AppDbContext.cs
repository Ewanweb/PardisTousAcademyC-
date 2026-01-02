using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using Pardis.Domain.Accounting;
using Pardis.Domain.Comments;
using Pardis.Domain.Attendance;
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
        public DbSet<ManualPaymentRequest> ManualPaymentRequests { get; set; }
        
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

            // ✅ تنظیمات ManualPaymentRequest
            builder.Entity<ManualPaymentRequest>()
                .HasOne(m => m.Course)
                .WithMany()
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ManualPaymentRequest>()
                .HasOne(m => m.Student)
                .WithMany()
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ManualPaymentRequest>()
                .HasOne(m => m.AdminReviewer)
                .WithMany()
                .HasForeignKey(m => m.AdminReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);

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
