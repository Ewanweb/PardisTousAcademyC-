using System.ComponentModel.DataAnnotations.Schema;

namespace Pardis.Domain.Courses
{
    public class CourseSection : BaseEntity
    {
        public string Title { get; set; } 
        public string Description { get; set; } 
        public int Order { get; set; } 
        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public CourseSection(string title, string description, int order, Guid courseId)
        {
            Title = title;
            Description = description;
            Order = order;
            CourseId = courseId;
        }
    }

}
