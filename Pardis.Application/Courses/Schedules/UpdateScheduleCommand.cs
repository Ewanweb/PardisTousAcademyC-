using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Application.Courses.Schedules;

/// <summary>
/// Command برای ویرایش زمان‌بندی دوره
/// </summary>
public class UpdateScheduleCommand : IRequest<OperationResult<CourseScheduleDto>>
{
    public Guid ScheduleId { get; set; }
    public UpdateCourseScheduleDto Dto { get; set; }

    public UpdateScheduleCommand(Guid scheduleId, UpdateCourseScheduleDto dto)
    {
        ScheduleId = scheduleId;
        Dto = dto;
    }
}