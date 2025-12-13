using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;

namespace Pardis.Application.Courses.Schedules;

public class EnrollInScheduleHandler : IRequestHandler<EnrollInScheduleCommand, OperationResult>
{
    private readonly IRepository<CourseSchedule> _scheduleRepository;
    private readonly IRepository<UserCourseSchedule> _userScheduleRepository;
    private readonly IRepository<UserCourse> _userCourseRepository;

    public EnrollInScheduleHandler(
        IRepository<CourseSchedule> scheduleRepository,
        IRepository<UserCourseSchedule> userScheduleRepository,
        IRepository<UserCourse> userCourseRepository)
    {
        _scheduleRepository = scheduleRepository;
        _userScheduleRepository = userScheduleRepository;
        _userCourseRepository = userCourseRepository;
    }

    public async Task<OperationResult> Handle(EnrollInScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود زمان‌بندی
        var schedule = await _scheduleRepository.Table
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == request.CourseScheduleId, cancellationToken);

        if (schedule == null)
            return OperationResult.NotFound("زمان‌بندی یافت نشد");

        if (!schedule.IsActive)
            return OperationResult.Error("این زمان‌بندی غیرفعال است");

        // 2. بررسی ظرفیت
        if (!schedule.HasCapacity)
            return OperationResult.Error("ظرفیت این زمان‌بندی تکمیل شده است");

        // 3. بررسی ثبت‌نام کاربر در دوره
        var userCourse = await _userCourseRepository.AnyAsync(uc => 
            uc.UserId == request.UserId && uc.CourseId == schedule.CourseId, 
            cancellationToken);

        if (!userCourse)
            return OperationResult.Error("ابتدا باید در دوره ثبت‌نام کنید");

        // 4. بررسی ثبت‌نام قبلی در این زمان‌بندی
        var alreadyEnrolled = await _userScheduleRepository.AnyAsync(ucs => 
            ucs.UserId == request.UserId && ucs.CourseScheduleId == request.CourseScheduleId,
            cancellationToken);

        if (alreadyEnrolled)
            return OperationResult.Error("شما قبلاً در این زمان‌بندی ثبت‌نام کرده‌اید");

        // 5. بررسی تداخل زمانی با سایر دوره‌ها
        var hasTimeConflict = await _userScheduleRepository.Table
            .Include(ucs => ucs.CourseSchedule)
            .AnyAsync(ucs => 
                ucs.UserId == request.UserId &&
                ucs.Status == StudentScheduleStatus.Active &&
                ucs.CourseSchedule.DayOfWeek == schedule.DayOfWeek &&
                ucs.CourseSchedule.StartTime < schedule.EndTime &&
                ucs.CourseSchedule.EndTime > schedule.StartTime,
                cancellationToken);

        if (hasTimeConflict)
            return OperationResult.Error("این زمان با دوره‌های دیگر شما تداخل دارد");

        // 6. ثبت‌نام در زمان‌بندی
        var userSchedule = new UserCourseSchedule
        {
            UserId = request.UserId,
            CourseScheduleId = request.CourseScheduleId,
            User = null!, // EF will handle this
            CourseSchedule = null! // EF will handle this
        };
        
        await _userScheduleRepository.AddAsync(userSchedule);

        // 7. افزایش شمارنده ثبت‌نام‌ها
        schedule.EnrolledCount++;
        _scheduleRepository.Update(schedule);

        await _userScheduleRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Success("ثبت‌نام در زمان‌بندی با موفقیت انجام شد");
    }
}