using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Domain.Users;

namespace Pardis.Infrastructure;

public static class RoleSeeder
{
    public async static Task SeedAsync(IServiceProvider serviceProvider)
    {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            var roleFields = typeof(Role).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            foreach (var field in roleFields)
            {
                // حل خطای CS8600 (Nullable): استفاده از عملگر ! یا چک کردن نال
                var roleName = (string?)field.GetValue(null);

                if (!string.IsNullOrEmpty(roleName) && !await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role(roleName));
                }
            }
    }
}
