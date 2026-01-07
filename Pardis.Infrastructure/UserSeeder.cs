using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Users;

namespace Pardis.Infrastructure
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // تعریف اطلاعات کاربر ادمین
            var adminEmail = "admin@pardis.com";

            // 1. بررسی وجود کاربر ادمین
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // ساخت آبجکت کاربر ادمین
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "مدیر سیستم",
                    Mobile = "09120000000",
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                // 2. ذخیره کاربر با رمز عبور
                var result = await userManager.CreateAsync(adminUser, "123456");

                if (result.Succeeded)
                {
                    // 3. تخصیص نقش‌ها (Admin و Manager)
                    await userManager.AddToRoleAsync(adminUser, Role.Admin);
                    await userManager.AddToRoleAsync(adminUser, Role.Manager);
                }
            }
            else
            {
                // اگر کاربر از قبل بود اما نقش نداشت، به او نقش بده
                if (!await userManager.IsInRoleAsync(adminUser, Role.Manager))
                {
                    await userManager.AddToRoleAsync(adminUser, Role.Manager);
                }

                if (!await userManager.IsInRoleAsync(adminUser, Role.Admin))
                {
                    await userManager.AddToRoleAsync(adminUser, Role.Admin);
                }
            }

            // تعریف اطلاعات کاربر عادی برای تست
            var testUserMobile = "09123456789";

            // بررسی وجود کاربر تست
            var testUser = await userManager.Users.FirstOrDefaultAsync(u => u.Mobile == testUserMobile);

            if (testUser == null)
            {
                // ساخت کاربر تست
                testUser = new User
                {
                    UserName = testUserMobile,
                    Email = "testuser@pardis.com",
                    FullName = "کاربر تست",
                    Mobile = testUserMobile,
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                // ذخیره کاربر تست با رمز عبور
                var testResult = await userManager.CreateAsync(testUser, "123456");

                if (testResult.Succeeded)
                {
                    // تخصیص نقش User
                    await userManager.AddToRoleAsync(testUser, Role.User);
                }
            }
            else
            {
                // اگر کاربر تست از قبل بود اما نقش نداشت
                if (!await userManager.IsInRoleAsync(testUser, Role.User))
                {
                    await userManager.AddToRoleAsync(testUser, Role.User);
                }
            }
        }
    }
}
