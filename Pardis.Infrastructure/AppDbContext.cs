using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Pardis.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // تنظیمات Soft Delete (فیلتر گلوبال)
            builder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);

            // تنظیمات سئو (به عنوان بخشی از جدول اصلی)
            builder.Entity<Category>().OwnsOne(c => c.Seo);
            builder.Entity<Course>().OwnsOne(c => c.Seo);
        }
    }
}
