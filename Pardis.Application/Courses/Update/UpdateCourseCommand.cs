using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Courses.Update
{
    public class UpdateCourseCommand() : IRequest<OperationResult<CourseResource>>
    {
        public UpdateCourseDto Dto { get; set; }
        public string CurrentUserId { get; set; } = string.Empty;
    }
}
