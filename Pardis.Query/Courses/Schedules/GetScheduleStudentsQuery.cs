using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Schedules;

public record GetScheduleStudentsQuery(Guid CourseScheduleId) : IRequest<List<ScheduleStudentDto>>;