using MediatR;

namespace Pardis.Query.Courses.GetCoursesByCategory
{
    public class GetCoursesByCategoryQuery : IRequest<object>
    {
        public Guid CategoryId { get; set; }
        public bool IsAdminOrManager { get; set; }
    }
}
