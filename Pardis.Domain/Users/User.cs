using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;

namespace Pardis.Domain.Users
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Mobile { get; set; }
        public string? Avatar { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Relationships
        public ICollection<Course> Courses { get; set; } // Courses taught by this user
        public ICollection<UserCourse> EnrolledCourses { get; set; }
        public ICollection<Category> CreatedCategories { get; set; }

        public User()
        {
            
        }

    }
}
