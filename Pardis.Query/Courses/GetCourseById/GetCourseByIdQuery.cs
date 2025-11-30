using MediatR;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<CourseResource>
    {
        public Guid Id { get; set; }
    }
}
