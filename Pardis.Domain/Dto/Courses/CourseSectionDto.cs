using System.ComponentModel.DataAnnotations;

namespace Pardis.Domain.Dto.Courses
{
    public class CourseSectionDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "عنوان بخش الزامی است")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "توضیحات بخش الزامی است")]
        public string Description { get; set; } = string.Empty;
        
        public int Order { get; set; }
    }
}