using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Query.Attendance;

/// <summary>
/// Query دریافت جلسات یک زمان‌بندی خاص
/// </summary>
public class GetSessionsByScheduleQuery : IRequest<List<CourseSessionDto>>
{
    public Guid ScheduleId { get; set; }
}