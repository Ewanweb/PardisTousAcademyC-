using MediatR;

namespace Pardis.Query.Courses.GetCoursesByCategory
{
    public class GetCoursesByCategoryQuery : IRequest<object>
    {
        public string Slug { get; set; }
        public bool IsAdminOrManager { get; set; }
    }
}
