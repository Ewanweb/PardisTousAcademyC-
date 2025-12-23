using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;

namespace Pardis.Application.Attendance;

/// <summary>
/// Handler برای بروزرسانی حضور و غیاب
/// </summary>
public class UpdateAttendanceHandler : IRequestHandler<UpdateAttendanceCommand, StudentAttendanceDto?>
{
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public UpdateAttendanceHandler(IStudentAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<StudentAttendanceDto?> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(request.AttendanceId);
        if (attendance == null)
            return null;

        attendance.UpdateAttendance(request.Status, request.UpdatedByUserId, request.Note);

        _attendanceRepository.Update(attendance);
        await _attendanceRepository.SaveChangesAsync(cancellationToken);

        return new StudentAttendanceDto
        {
            Id = attendance.Id,
            SessionId = attendance.SessionId,
            SessionTitle = attendance.Session?.Title ?? "نامشخص",
            SessionDate = attendance.Session?.SessionDate ?? DateTime.MinValue,
            StudentId = attendance.StudentId,
            StudentName = attendance.Student?.FullName ?? attendance.Student?.UserName ?? "نامشخص",
            Status = attendance.Status,
            StatusDisplay = GetAttendanceStatusDisplay(attendance.Status),
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            AttendanceDuration = attendance.GetAttendanceDuration(),
            Note = attendance.Note,
            RecordedByUserName = attendance.RecordedByUser?.FullName ?? attendance.RecordedByUser?.UserName,
            CreatedAt = attendance.CreatedAt,
            UpdatedAt = attendance.UpdatedAt
        };
    }

    private static string GetAttendanceStatusDisplay(AttendanceStatus status)
    {
        return status switch
        {
            AttendanceStatus.Present => "حاضر",
            AttendanceStatus.Absent => "غایب",
            AttendanceStatus.Late => "تأخیر",
            _ => status.ToString()
        };
    }
}