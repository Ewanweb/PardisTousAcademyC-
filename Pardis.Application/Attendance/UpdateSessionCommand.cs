using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Application.Attendance;

public class UpdateSessionCommand : IRequest<CourseSessionDto>
{
    public Guid SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public TimeSpan Duration { get; set; }
}