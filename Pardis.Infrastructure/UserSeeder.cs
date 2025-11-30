using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Infrastructure
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // تعریف اطلاعات کاربر ادمین
            var adminEmail = "admin@pardis.com";

            // 1. بررسی وجود کاربر
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // ساخت آبجکت کاربر
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
        }
    }
}
