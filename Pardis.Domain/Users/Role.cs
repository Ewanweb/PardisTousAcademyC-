using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pardis.Domain.Users
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; } // مثلا توضیحات فارسی نقش
        // نقش‌های پایه
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User"; // یا Student

        // نقش‌های سازمانی (ترجمه شده موارد درخواستی شما)
        public const string FinancialManager = "FinancialManager"; // مدیر مالی
        public const string Instructor = "Instructor";             // مدرس
        public const string Student = "Student";                   // دانشجو
        public const string ITManager = "ITManager";               // مدیر آیتی
        public const string MarketingManager = "MarketingManager"; // مدیر مارکتینگ
        public const string EducationManager = "EducationManager"; // مدیر آموزش
        public const string Accountant = "Accountant";             // حسابدار
        public const string GeneralManager = "GeneralManager";     // مدیر کل
        public const string DepartmentManager = "DepartmentManager"; // مدیر دپارتمان
        public const string CourseSupport = "CourseSupport";       // پشتیبان دوره
        public const string Marketer = "Marketer";                 // بازاریاب
        public const string InternalManager = "InternalManager";   // مدیر داخلی
        public const string EducationExpert = "EducationExpert";   // کارشناس آموزش
        public Role() : base() { }
        public Role(string roleName) : base(roleName) { }
    }
}
