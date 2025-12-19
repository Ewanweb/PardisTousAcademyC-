using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Schedules;

/// <summary>
/// Handler برای دریافت یک زمان‌بندی خاص
/// </summary>
public class GetScheduleByIdHandler : IRequestHandler<GetScheduleByIdQuery, CourseScheduleDto?>
{
    private readonly IRepository<CourseSchedule> _scheduleRepository;

    public GetScheduleByIdHandler(IRepository<CourseSchedule> scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<CourseScheduleDto?> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.Table
            .AsNoTracking()
            .Include(s => s.StudentEnrollments)
            .FirstOrDefaultAsync(s => s.Id == request.ScheduleId, cancellationToken);

        if (schedule == null)
            return null;

        var enrolledCount = schedule.StudentEnrollments?.Count(e => e.Status == StudentScheduleStatus.Active) ?? 0;

        return new CourseScheduleDto
        {
            Id = schedule.Id,
            CourseId = schedule.CourseId,
            Title = schedule.Title,
            DayOfWeek = schedule.DayOfWeek,
            DayName = schedule.GetDayName(),
            StartTime = schedule.StartTime.ToString("HH:mm"),
            EndTime = schedule.EndTime.ToString("HH:mm"),
            TimeRange = $"{schedule.StartTime:HH:mm}-{schedule.EndTime:HH:mm}",
            FullScheduleText = schedule.GetFullScheduleText(),
            MaxCapacity = schedule.MaxCapacity,
            EnrolledCount = enrolledCount,
            RemainingCapacity = schedule.MaxCapacity - enrolledCount,
            HasCapacity = schedule.MaxCapacity > enrolledCount,
            IsActive = schedule.IsActive,
            Description = schedule.Description,
            CreatedAt = schedule.CreatedAt
        };
    }
}