using MediatR;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Courses;

namespace Pardis.Query.Courses;

/// <summary>
/// Handler برای دریافت زمان‌بندی‌های دوره
/// </summary>
public class GetCourseSchedulesHandler : IRequestHandler<GetCourseSchedulesQuery, List<CourseScheduleDto>>
{
    private readonly ICourseScheduleRepository _scheduleRepository;

    public GetCourseSchedulesHandler(ICourseScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<List<CourseScheduleDto>> Handle(GetCourseSchedulesQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetSchedulesByCourseIdAsync(request.CourseId, cancellationToken);

        return schedules.Select(schedule => new CourseScheduleDto
        {
            Id = schedule.Id,
            CourseId = schedule.CourseId,
            Title = schedule.Title,
            DayOfWeek = schedule.DayOfWeek,
            DayName = schedule.GetDayName(),
            StartTime = schedule.StartTime.ToString("HH:mm"),
            EndTime = schedule.EndTime.ToString("HH:mm"),
            FullScheduleText = schedule.GetFullScheduleText(),
            MaxCapacity = schedule.MaxCapacity,
            EnrolledCount = schedule.EnrolledCount,
            RemainingCapacity = schedule.RemainingCapacity,
            HasCapacity = schedule.HasCapacity,
            IsActive = schedule.IsActive,
            Description = schedule.Description,
            CreatedAt = schedule.CreatedAt
        }).ToList();
    }
}