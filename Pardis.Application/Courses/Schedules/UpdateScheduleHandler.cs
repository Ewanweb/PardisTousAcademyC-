using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Application.Courses.Schedules;

/// <summary>
/// Handler برای ویرایش زمان‌بندی دوره
/// </summary>
public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, OperationResult<CourseScheduleDto>>
{
    private readonly IRepository<CourseSchedule> _scheduleRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper;

    public UpdateScheduleHandler(
        IRepository<CourseSchedule> scheduleRepository,
        IRepository<Course> courseRepository,
        IMapper mapper)
    {
        _scheduleRepository = scheduleRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CourseScheduleDto>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // بررسی وجود زمان‌بندی
            var schedule = await _scheduleRepository.Table
                .Include(s => s.StudentEnrollments)
                .FirstOrDefaultAsync(s => s.Id == request.ScheduleId, cancellationToken);

            if (schedule == null)
            {
                return OperationResult<CourseScheduleDto>.NotFound("زمان‌بندی یافت نشد");
            }

            // بررسی وجود دوره
            var courseExists = await _courseRepository.Table
                .AnyAsync(c => c.Id == schedule.CourseId && !c.IsDeleted, cancellationToken);

            if (!courseExists)
            {
                return OperationResult<CourseScheduleDto>.Error("دوره مربوطه یافت نشد یا حذف شده است");
            }

            // تبدیل و اعتبارسنجی زمان‌ها
            if (!TimeOnly.TryParse(request.Dto.StartTime, out var startTime))
            {
                return OperationResult<CourseScheduleDto>.Error("فرمت ساعت شروع نامعتبر است");
            }

            if (!TimeOnly.TryParse(request.Dto.EndTime, out var endTime))
            {
                return OperationResult<CourseScheduleDto>.Error("فرمت ساعت پایان نامعتبر است");
            }

            if (startTime >= endTime)
            {
                return OperationResult<CourseScheduleDto>.Error("ساعت پایان باید بزرگتر از ساعت شروع باشد");
            }

            // اعتبارسنجی روز هفته
            if (request.Dto.DayOfWeek < 0 || request.Dto.DayOfWeek > 6)
            {
                return OperationResult<CourseScheduleDto>.Error("روز هفته نامعتبر است (0-6)");
            }

            // اعتبارسنجی ظرفیت
            if (request.Dto.MaxCapacity < 1)
            {
                return OperationResult<CourseScheduleDto>.Error("ظرفیت باید حداقل 1 نفر باشد");
            }

            // بررسی اینکه ظرفیت جدید کمتر از تعداد ثبت‌نام‌شدگان فعلی نباشد
            var currentEnrollments = schedule.StudentEnrollments?.Count(e => e.Status == StudentScheduleStatus.Active) ?? 0;
            if (request.Dto.MaxCapacity < currentEnrollments)
            {
                return OperationResult<CourseScheduleDto>.Error($"ظرفیت نمی‌تواند کمتر از تعداد ثبت‌نام‌شدگان فعلی ({currentEnrollments} نفر) باشد");
            }

            // بررسی تداخل زمانی با سایر زمان‌بندی‌های همین دوره
            var hasTimeConflict = await _scheduleRepository.Table
                .AnyAsync(s => s.CourseId == schedule.CourseId && 
                              s.Id != request.ScheduleId &&
                              s.IsActive &&
                              s.DayOfWeek == request.Dto.DayOfWeek &&
                              ((startTime >= s.StartTime && startTime < s.EndTime) ||
                               (endTime > s.StartTime && endTime <= s.EndTime) ||
                               (startTime <= s.StartTime && endTime >= s.EndTime)), 
                          cancellationToken);

            if (hasTimeConflict)
            {
                return OperationResult<CourseScheduleDto>.Error("زمان‌بندی با زمان‌بندی دیگری از همین دوره تداخل دارد");
            }

            // به‌روزرسانی فیلدها
            schedule.Title = request.Dto.Title;
            schedule.DayOfWeek = request.Dto.DayOfWeek;
            schedule.StartTime = startTime;
            schedule.EndTime = endTime;
            schedule.MaxCapacity = request.Dto.MaxCapacity;
            schedule.IsActive = request.Dto.IsActive;
            schedule.Description = request.Dto.Description;
            schedule.UpdatedAt = DateTime.UtcNow;

            // ذخیره تغییرات
            await _scheduleRepository.SaveChangesAsync(cancellationToken);

            // تبدیل به DTO
            var result = new CourseScheduleDto
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
                EnrolledCount = currentEnrollments,
                RemainingCapacity = schedule.MaxCapacity - currentEnrollments,
                HasCapacity = schedule.MaxCapacity > currentEnrollments,
                IsActive = schedule.IsActive,
                Description = schedule.Description,
                CreatedAt = schedule.CreatedAt
            };

            return OperationResult<CourseScheduleDto>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<CourseScheduleDto>.Error($"خطا در ویرایش زمان‌بندی: {ex.Message}");
        }
    }
}