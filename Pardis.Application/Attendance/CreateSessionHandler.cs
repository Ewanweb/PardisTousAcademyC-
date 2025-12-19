using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;
using Pardis.Domain.Courses;
using Pardis.Domain;
using Microsoft.EntityFrameworkCore;

namespace Pardis.Application.Attendance;

/// <summary>
/// Handler برای ایجاد جلسه جدید
/// </summary>
public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, CourseSessionDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly ICourseScheduleRepository _scheduleRepository;
    private readonly IRepository<UserCourseSchedule> _userScheduleRepository;

    public CreateSessionHandler(
        ICourseRepository courseRepository,
        ICourseSessionRepository sessionRepository,
        ICourseScheduleRepository scheduleRepository,
        IRepository<UserCourseSchedule> userScheduleRepository)
    {
        _courseRepository = courseRepository;
        _sessionRepository = sessionRepository;
        _scheduleRepository = scheduleRepository;
        _userScheduleRepository = userScheduleRepository;
    }

    public async Task<CourseSessionDto> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new ArgumentException("دوره یافت نشد", nameof(request.CourseId));

        if (request.ScheduleId.HasValue && request.ScheduleId.Value != Guid.Empty)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId.Value);
            if (schedule == null)
                throw new ArgumentException("زمان‌بندی یافت نشد", nameof(request.ScheduleId));

            if (schedule.CourseId != request.CourseId)
                throw new ArgumentException("زمان‌بندی انتخاب شده متعلق به این دوره نیست", nameof(request.ScheduleId));
        }

        // بررسی تکراری نبودن شماره جلسه در زمان‌بندی
        var duplicateExists = await _sessionRepository.AnyAsync(
            s => s.ScheduleId == request.ScheduleId && s.SessionNumber == request.SessionNumber,
            cancellationToken);
        
        if (duplicateExists)
            throw new ArgumentException($"جلسه با شماره {request.SessionNumber} قبلاً برای این زمان‌بندی ثبت شده است. لطفاً شماره جلسه دیگری انتخاب کنید.", nameof(request.SessionNumber));

        var session = new CourseSession(
            request.CourseId,
            request.Title,
            request.SessionDate,
            request.Duration,
            request.SessionNumber,
            null, // description
            request.ScheduleId);

        await _sessionRepository.AddAsync(session);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        // دریافت دانشجویان زمان‌بندی برای حضور و غیاب
        var students = new List<CourseStudentDto>();
        if (request.ScheduleId.HasValue && request.ScheduleId.Value != Guid.Empty)
        {
            students = await _userScheduleRepository.Table
                .Include(ucs => ucs.User)
                .Where(ucs => ucs.CourseScheduleId == request.ScheduleId.Value)
                .OrderBy(ucs => ucs.User.FullName)
                .Select(ucs => new CourseStudentDto
                {
                    StudentId = ucs.UserId,
                    StudentName = ucs.User.FullName ?? ucs.User.UserName ?? "",
                    StudentEmail = ucs.User.Email,
                    StudentMobile = ucs.User.PhoneNumber,
                    EnrollmentDate = ucs.EnrolledAt,
                    CurrentSessionStatus = null,
                    CurrentSessionStatusDisplay = null,
                    CheckInTime = null,
                    Note = null
                })
                .ToListAsync(cancellationToken);
        }

        return new CourseSessionDto
        {
            Id = session.Id,
            CourseId = session.CourseId,
            ScheduleId = session.ScheduleId,
            CourseName = course.Title,
            Title = session.Title,
            Description = session.Description,
            SessionDate = session.SessionDate,
            Duration = session.Duration,
            SessionNumber = session.SessionNumber,
            Status = session.Status,
            StatusDisplay = GetSessionStatusDisplay(session.Status),
            TotalStudents = students.Count,
            PresentStudents = 0,
            AbsentStudents = 0,
            LateStudents = 0,
            CreatedAt = session.CreatedAt,
            Students = students
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