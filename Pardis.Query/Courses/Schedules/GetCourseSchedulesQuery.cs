using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Schedules;

/// <summary>
/// Query برای دریافت لیست زمان‌بندی‌های یک دوره
/// </summary>
public record GetCourseSchedulesQuery(Guid CourseId) : IRequest<List<CourseScheduleDto>>;