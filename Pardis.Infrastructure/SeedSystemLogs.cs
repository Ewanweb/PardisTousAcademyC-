using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Logging;

namespace Pardis.Infrastructure;

public static class SystemLogSeeder
{
    public static async Task SeedSampleLogs(AppDbContext context)
    {
        if (await context.SystemLogs.AnyAsync())
            return; // Already seeded

        var logs = new List<SystemLog>
        {
            // لاگ‌های ثبت‌نام کاربر
            new SystemLog(
                DateTime.UtcNow.AddDays(-5),
                "success",
                "Auth",
                "کاربر جدید 'علی احمدی' با موفقیت ثبت‌نام کرد",
                "user-001",
                null,
                "UserRegistered",
                "Email: ali@example.com, Mobile: 09121234567"
            ),
            new SystemLog(
                DateTime.UtcNow.AddDays(-4),
                "success",
                "Auth",
                "کاربر جدید 'سارا محمدی' با موفقیت ثبت‌نام کرد",
                "user-002",
                null,
                "UserRegistered",
                "Email: sara@example.com, Mobile: 09129876543"
            ),

            // لاگ‌های ایجاد دوره
            new SystemLog(
                DateTime.UtcNow.AddDays(-3),
                "success",
                "Course",
                "دوره جدید 'آموزش جامع React' ایجاد شد",
                "admin-001",
                null,
                "CourseCreated",
                "Price: 2500000, Category: برنامه‌نویسی وب"
            ),
            new SystemLog(
                DateTime.UtcNow.AddDays(-2),
                "success",
                "Course",
                "دوره جدید 'طراحی UI/UX پیشرفته' ایجاد شد",
                "admin-001",
                null,
                "CourseCreated",
                "Price: 1800000, Category: طراحی"
            ),

            // لاگ‌های خرید دوره
            new SystemLog(
                DateTime.UtcNow.AddDays(-1).AddHours(-5),
                "success",
                "Payment",
                "پرداخت به مبلغ 2,500,000 تومان توسط ادمین تایید شد",
                "user-001",
                "req-12345",
                "PaymentApproved",
                "OrderId: order-001, AdminId: admin-001"
            ),
            new SystemLog(
                DateTime.UtcNow.AddDays(-1).AddHours(-4),
                "success",
                "Enrollment",
                "کاربر در دوره 'آموزش جامع React' ثبت‌نام شد",
                "user-001",
                null,
                "CourseEnrolled",
                "CourseId: course-001, Price: 2,500,000"
            ),

            // لاگ‌های بیشتر برای خرید
            new SystemLog(
                DateTime.UtcNow.AddHours(-10),
                "success",
                "Payment",
                "پرداخت به مبلغ 1,800,000 تومان توسط ادمین تایید شد",
                "user-002",
                "req-12346",
                "PaymentApproved",
                "OrderId: order-002, AdminId: admin-001"
            ),
            new SystemLog(
                DateTime.UtcNow.AddHours(-9),
                "success",
                "Enrollment",
                "کاربر در دوره 'طراحی UI/UX پیشرفته' ثبت‌نام شد",
                "user-002",
                null,
                "CourseEnrolled",
                "CourseId: course-002, Price: 1,800,000"
            ),

            // لاگ‌های رد پرداخت
            new SystemLog(
                DateTime.UtcNow.AddHours(-6),
                "warning",
                "Payment",
                "پرداخت به مبلغ 3,200,000 تومان توسط ادمین رد شد. دلیل: رسید نامعتبر است",
                "user-003",
                "req-12347",
                "PaymentRejected",
                "AdminId: admin-001"
            ),

            // لاگ‌های ورود کاربران
            new SystemLog(
                DateTime.UtcNow.AddHours(-3),
                "info",
                "Auth",
                "کاربر 'علی احمدی' وارد سیستم شد",
                "user-001",
                null,
                "UserLogin",
                "IP: 192.168.1.100"
            ),
            new SystemLog(
                DateTime.UtcNow.AddHours(-2),
                "info",
                "Auth",
                "کاربر 'سارا محمدی' وارد سیستم شد",
                "user-002",
                null,
                "UserLogin",
                "IP: 192.168.1.101"
            ),

            // لاگ‌های به‌روزرسانی تنظیمات
            new SystemLog(
                DateTime.UtcNow.AddHours(-1),
                "info",
                "System",
                "تنظیمات سیستم به‌روزرسانی شد",
                "admin-001",
                null,
                "SettingsUpdated",
                "Keys: System.SiteName, System.SiteLogo"
            ),

            // لاگ‌های خطا
            new SystemLog(
                DateTime.UtcNow.AddMinutes(-30),
                "error",
                "Email",
                "خطا در ارسال ایمیل به کاربر",
                "user-004",
                null,
                "EmailSendFailed",
                "Error: SMTP connection timeout"
            ),

            // لاگ‌های اخیر
            new SystemLog(
                DateTime.UtcNow.AddMinutes(-15),
                "success",
                "Course",
                "دوره جدید 'آموزش Python مقدماتی' ایجاد شد",
                "admin-001",
                null,
                "CourseCreated",
                "Price: 1500000, Category: برنامه‌نویسی"
            ),
            new SystemLog(
                DateTime.UtcNow.AddMinutes(-5),
                "info",
                "Auth",
                "کاربر 'محمد رضایی' وارد سیستم شد",
                "user-005",
                null,
                "UserLogin",
                "IP: 192.168.1.102"
            )
        };

        await context.SystemLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
    }
}
