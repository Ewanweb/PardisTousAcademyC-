using MediatR;
using Pardis.Domain.Attendance;

namespace Pardis.Application.Attendance;

/// <summary>
/// Handler برای حذف جلسه
/// </summary>
public class DeleteSessionHandler : IRequestHandler<DeleteSessionCommand, bool>
{
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public DeleteSessionHandler(
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository)
    {
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<bool> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null)
            return false;

        // حذف تمام حضور و غیاب‌های مربوط به این جلسه
        var attendances = await _attendanceRepository.GetAttendancesBySessionIdAsync(request.SessionId, cancellationToken);
        if (attendances.Any())
        {
            _attendanceRepository.RemoveRange(attendances);
        }

        // حذف جلسه
        _sessionRepository.Remove(session);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}