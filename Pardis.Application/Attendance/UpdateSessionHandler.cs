using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;

namespace Pardis.Application.Attendance;

/// <summary>
/// Handler برای بروزرسانی جلسه
/// </summary>
public class UpdateSessionHandler : IRequestHandler<UpdateSessionCommand, CourseSessionDto>
{
    private readonly ICourseSessionRepository _sessionRepository;

    public UpdateSessionHandler(ICourseSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<CourseSessionDto> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null)
            throw new ArgumentException("جلسه یافت نشد", nameof(request.SessionId));

        session.UpdateSession(request.Title, request.SessionDate, request.Duration);

        _sessionRepository.Update(session);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return new CourseSessionDto
        {
            Id = session.Id,
            CourseId = session.CourseId,
            CourseName = session.Course?.Title ?? "نامشخص",
            Title = session.Title,
            Description = session.Description,
            SessionDate = session.SessionDate,
            Duration = session.Duration,
            SessionNumber = session.SessionNumber,
            Status = session.Status,
            StatusDisplay = GetSessionStatusDisplay(session.Status),
            TotalStudents = 0,
            PresentStudents = 0,
            AbsentStudents = 0,
            LateStudents = 0,
            CreatedAt = session.CreatedAt
        };
    }

    private static string GetSessionStatusDisplay(SessionStatus status)
    {
        return status switch
        {
            SessionStatus.Scheduled => "زمان‌بندی شده",
            SessionStatus.InProgress => "در حال برگزاری",
            SessionStatus.Completed => "تکمیل شده",
            SessionStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }
}