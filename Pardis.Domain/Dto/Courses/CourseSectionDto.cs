namespace Pardis.Domain.Dto.Courses
{
    public class CourseSectionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } 
        public int Order { get; set; }
    }
}