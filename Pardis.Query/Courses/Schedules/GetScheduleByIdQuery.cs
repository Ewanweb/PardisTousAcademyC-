using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Schedules;

/// <summary>
/// Query برای دریافت یک زمان‌بندی خاص
/// </summary>
public record GetScheduleByIdQuery(Guid ScheduleId) : IRequest<CourseScheduleDto?>;