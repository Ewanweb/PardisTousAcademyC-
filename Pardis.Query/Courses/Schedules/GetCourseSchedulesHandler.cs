using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Schedules;

/// <summary>
/// Handler برای دریافت لیست زمان‌بندی‌های یک دوره
/// </summary>
public class GetCourseSchedulesHandler : IRequestHandler<GetCourseSchedulesQuery, List<CourseScheduleDto>>
{
    private readonly IRepository<CourseSchedule> _scheduleRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper;

    public GetCourseSchedulesHandler(
        IRepository<CourseSchedule> scheduleRepository,
        IRepository<Course> courseRepository,
        IMapper mapper)
    {
        _scheduleRepository = scheduleRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<CourseScheduleDto>> Handle(GetCourseSchedulesQuery request, CancellationToken cancellationToken)
    {
        // بررسی وجود دوره
        var courseExists = await _courseRepository.Table
            .AnyAsync(c => c.Id == request.CourseId && !c.IsDeleted, cancellationToken);

        if (!courseExists)
        {
            return new List<CourseScheduleDto>();
        }

        // دریافت زمان‌بندی‌های فعال دوره
        var schedules = await _scheduleRepository.Table
            .AsNoTracking()
            .Include(s => s.StudentEnrollments) // برای محاسبه تعداد ثبت‌نام‌شدگان
            .Where(s => s.CourseId == request.CourseId && s.IsActive)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        // تبدیل به DTO با محاسبه تعداد ثبت‌نام‌شدگان
        var result = schedules.Select(s => new CourseScheduleDto
        {
            Id = s.Id,
            CourseId = s.CourseId,
            Title = s.Title,
            DayOfWeek = s.DayOfWeek,
            DayName = s.GetDayName(),
            StartTime = s.StartTime.ToString("HH:mm"),
            EndTime = s.EndTime.ToString("HH:mm"),
            TimeRange = $"{s.StartTime:HH:mm}-{s.EndTime:HH:mm}",
            FullScheduleText = s.GetFullScheduleText(),
            MaxCapacity = s.MaxCapacity,
            EnrolledCount = s.StudentEnrollments?.Count(e => e.Status == StudentScheduleStatus.Active) ?? 0,
            RemainingCapacity = s.MaxCapacity - (s.StudentEnrollments?.Count(e => e.Status == StudentScheduleStatus.Active) ?? 0),
            HasCapacity = s.MaxCapacity > (s.StudentEnrollments?.Count(e => e.Status == StudentScheduleStatus.Active) ?? 0),
            IsActive = s.IsActive,
            Description = s.Description,
            CreatedAt = s.CreatedAt
        }).ToList();

        return result;
    }
}