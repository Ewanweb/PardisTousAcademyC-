using AutoMapper;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Application.Courses.Schedules;

public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, OperationResult<CourseScheduleDto>>
{
    private readonly IRepository<CourseSchedule> _scheduleRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper;

    public CreateScheduleHandler(IRepository<CourseSchedule> scheduleRepository, IRepository<Course> courseRepository, IMapper mapper)
    {
        _scheduleRepository = scheduleRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CourseScheduleDto>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود دوره
        var course = await _courseRepository.GetByIdAsync(request.Dto.CourseId);
        if (course == null)
            return OperationResult<CourseScheduleDto>.NotFound("دوره یافت نشد");

        // 2. تبدیل و اعتبارسنجی زمان
        if (!TimeOnly.TryParse(request.Dto.StartTime, out var startTime))
            return OperationResult<CourseScheduleDto>.Error("فرمت ساعت شروع نامعتبر است");

        if (!TimeOnly.TryParse(request.Dto.EndTime, out var endTime))
            return OperationResult<CourseScheduleDto>.Error("فرمت ساعت پایان نامعتبر است");

        if (startTime >= endTime)
            return OperationResult<CourseScheduleDto>.Error("ساعت شروع باید کمتر از ساعت پایان باشد");

        if (request.Dto.DayOfWeek < 0 || request.Dto.DayOfWeek > 6)
            return OperationResult<CourseScheduleDto>.Error("روز هفته نامعتبر است");

        if (request.Dto.MaxCapacity <= 0)
            return OperationResult<CourseScheduleDto>.Error("ظرفیت باید بیشتر از صفر باشد");

        // 3. بررسی تداخل زمانی
        var hasConflict = await _scheduleRepository.AnyAsync(s => 
            s.CourseId == request.Dto.CourseId &&
            s.DayOfWeek == request.Dto.DayOfWeek &&
            ((s.StartTime < endTime && s.EndTime > startTime)), 
            cancellationToken);

        if (hasConflict)
            return OperationResult<CourseScheduleDto>.Error("این زمان با زمان‌بندی موجود تداخل دارد");

        // 4. ایجاد زمان‌بندی
        var schedule = new CourseSchedule
        {
            CourseId = request.Dto.CourseId,
            Course = null!, // EF will handle this
            Title = request.Dto.Title,
            DayOfWeek = request.Dto.DayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            MaxCapacity = request.Dto.MaxCapacity,
            Description = request.Dto.Description
        };

        await _scheduleRepository.AddAsync(schedule);
        await _scheduleRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<CourseScheduleDto>(schedule);
        return OperationResult<CourseScheduleDto>.Success(result);
    }
}