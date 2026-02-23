using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;

namespace Pardis.Application.Courses.Schedules;

/// <summary>
/// Handler برای ایجاد زمان‌بندی جدید
/// </summary>
public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, OperationResult<Guid>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseSchedule> _scheduleRepository;

    public CreateScheduleHandler(
        IRepository<Course> courseRepository,
        IRepository<CourseSchedule> scheduleRepository)
    {
        _courseRepository = courseRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<OperationResult<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // بررسی وجود دوره
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return OperationResult<Guid>.NotFound("دوره یافت نشد");
            }

            // اعتبارسنجی ورودی‌ها
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return OperationResult<Guid>.Error("عنوان زمان‌بندی الزامی است");
            }

            if (request.DayOfWeek < 0 || request.DayOfWeek > 6)
            {
                return OperationResult<Guid>.Error("روز هفته نامعتبر است (باید بین 0 تا 6 باشد)");
            }

            if (request.MaxCapacity <= 0)
            {
                return OperationResult<Guid>.Error("ظرفیت باید بیشتر از صفر باشد");
            }

            // تبدیل زمان‌ها
            if (!TimeOnly.TryParse(request.StartTime, out var startTime))
            {
                return OperationResult<Guid>.Error("فرمت ساعت شروع نامعتبر است (مثال: 12:00)");
            }

            if (!TimeOnly.TryParse(request.EndTime, out var endTime))
            {
                return OperationResult<Guid>.Error("فرمت ساعت پایان نامعتبر است (مثال: 14:00)");
            }

            if (endTime <= startTime)
            {
                return OperationResult<Guid>.Error("ساعت پایان باید بعد از ساعت شروع باشد");
            }

            // ایجاد زمان‌بندی جدید
            var schedule = new CourseSchedule
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                Course = course,
                Title = request.Title,
                DayOfWeek = request.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                MaxCapacity = request.MaxCapacity,
                Description = request.Description,
                IsActive = true,
                EnrolledCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _scheduleRepository.AddAsync(schedule);
            await _scheduleRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<Guid>.Success(schedule.Id);
        }
        catch (Exception ex)
        {
            return OperationResult<Guid>.Error($"خطا در ایجاد زمان‌بندی: {ex.Message}");
        }
    }
}
