using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Application.Attendance;

public class CreateSessionCommand : IRequest<CourseSessionDto>
{
    public Guid CourseId { get; set; }
    public Guid? ScheduleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public TimeSpan Duration { get; set; }
    public int SessionNumber { get; set; }
}