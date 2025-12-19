using MediatR;
using Pardis.Domain.Attendance;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Application.Attendance;

public class RecordAttendanceCommand : IRequest<StudentAttendanceDto>
{
    public Guid SessionId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public AttendanceStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Note { get; set; }
    public string RecordedByUserId { get; set; } = string.Empty;
}