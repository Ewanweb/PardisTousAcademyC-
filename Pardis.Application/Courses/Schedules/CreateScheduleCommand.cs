using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Application.Courses.Schedules;

public record CreateScheduleCommand(CreateCourseScheduleDto Dto) : IRequest<OperationResult<CourseScheduleDto>>;