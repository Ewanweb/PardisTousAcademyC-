using MediatR;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Query.Attendance;

/// <summary>
/// Query دریافت حضور و غیاب یک جلسه
/// </summary>
public class GetSessionAttendanceQuery : IRequest<SessionAttendanceDto?>
{
    public Guid SessionId { get; set; }
}