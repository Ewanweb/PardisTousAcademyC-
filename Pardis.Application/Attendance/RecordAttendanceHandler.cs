using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;
using Pardis.Domain.Users;

namespace Pardis.Application.Attendance;

/// <summary>
/// Handler برای ثبت حضور و غیاب
/// </summary>
public class RecordAttendanceHandler : IRequestHandler<RecordAttendanceCommand, StudentAttendanceDto>
{
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;
    private readonly IUserRepository _userRepository;

    public RecordAttendanceHandler(
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository,
        IUserRepository userRepository)
    {
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
        _userRepository = userRepository;
    }

    public async Task<StudentAttendanceDto> Handle(RecordAttendanceCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null)
            throw new ArgumentException("جلسه یافت نشد", nameof(request.SessionId));

        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            throw new ArgumentException("دانشجو یافت نشد", nameof(request.StudentId));

        // بررسی اینکه آیا قبلاً حضور و غیاب ثبت شده یا نه
        var existingAttendance = await _attendanceRepository.GetAttendanceBySessionAndStudentAsync(
            request.SessionId, request.StudentId, cancellationToken);

        if (existingAttendance != null)
            throw new ArgumentException($"حضور و غیاب این دانشجو قبلاً ثبت شده است. برای تغییر وضعیت از ویرایش استفاده کنید.", nameof(request.StudentId));

        // ایجاد حضور و غیاب جدید
        var attendance = StudentAttendance.Create(
            request.SessionId,
            request.StudentId,
            request.Status,
            request.CheckInTime,
            request.CheckOutTime,
            request.Note,
            request.RecordedByUserId);

        await _attendanceRepository.AddAsync(attendance);

        await _attendanceRepository.SaveChangesAsync(cancellationToken);

        return new StudentAttendanceDto
        {
            Id = attendance.Id,
            SessionId = attendance.SessionId,
            SessionTitle = session.Title,
            SessionDate = session.SessionDate,
            StudentId = attendance.StudentId,
            StudentName = student.FullName ?? student.UserName ?? "نامشخص",
            Status = attendance.Status,
            StatusDisplay = GetAttendanceStatusDisplay(attendance.Status),
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            AttendanceDuration = attendance.GetAttendanceDuration(),
            Note = attendance.Note,
            RecordedByUserName = null, // می‌توان بعداً اضافه کرد
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