using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.GetCourses
{
    public class GetCoursesQuery : IRequest<List<CourseResource>>
    {
        public bool Trashed { get; set; }
        public Guid? CategoryId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public string? CurrentUserId { get; set; }
        public bool IsAdminOrManager { get; set; }
        public bool IsInstructor { get; set; }
    }
}
