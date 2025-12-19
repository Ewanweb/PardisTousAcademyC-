using MediatR;
using Pardis.Domain.Attendance;
using Pardis.Domain.Dto.Attendance;

namespace Pardis.Application.Attendance;

/// <summary>
/// Command برای بروزرسانی حضور و غیاب
/// </summary>
public class UpdateAttendanceCommand : IRequest<StudentAttendanceDto>
{
    public Guid AttendanceId { get; set; }
    public AttendanceStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? Note { get; set; }
}