using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Courses.Schedules;

public record EnrollInScheduleCommand(Guid CourseScheduleId, string UserId) : IRequest<OperationResult>;