using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Domain.Users;

namespace Pardis.Infrastructure;

public static class RoleSeeder
{
    private static readonly Dictionary<string, string> PersianDescriptions = new()
        {
            { Role.Admin, "مدیر سیستم" },
            { Role.Manager, "مدیر ارشد" },
            { Role.User, "کاربر عادی" },
            { Role.Student, "دانشجو" },
            { Role.Instructor, "مدرس" },
            { Role.FinancialManager, "مدیر مالی" },
            { Role.ITManager, "مدیر فنی" },
            { Role.MarketingManager, "مدیر بازاریابی" },
            { Role.EducationManager, "مدیر آموزش" },
            { Role.Accountant, "حسابدار" },
            { Role.GeneralManager, "مدیر کل" },
            { Role.DepartmentManager, "مدیر دپارتمان" },
            { Role.CourseSupport, "پشتیبان دوره" },
            { Role.Marketer, "بازاریاب" },
            { Role.InternalManager, "مدیر داخلی" },
            { Role.EducationExpert, "کارشناس آموزش" }
        };

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var roleFields = typeof(Role).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

        foreach (var field in roleFields)
        {
            var roleName = (string)field.GetValue(null);
            var description = PersianDescriptions.ContainsKey(roleName) ? PersianDescriptions[roleName] : roleName;

            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                // ایجاد نقش جدید با توضیحات
                role = new Role(roleName) { Description = description };
                await roleManager.CreateAsync(role);
            }
            else if (role.Description != description)
            {
                // آپدیت توضیحات اگر قبلا وجود داشته ولی توضیحاتش فرق دارد
                role.Description = description;
                await roleManager.UpdateAsync(role);
            }
        }
    }
}
