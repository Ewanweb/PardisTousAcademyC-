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

        // Extended Profile Fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AvatarFileId { get; set; }
        public DateTime? AvatarUpdatedAt { get; set; }

        // Relationships
        public ICollection<Course> Courses { get; set; } = [];
        public ICollection<UserCourse> EnrolledCourses { get; set; } = [];
        public ICollection<Category> CreatedCategories { get; set; } = [];

        public User()
        {
            
        }
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
}
