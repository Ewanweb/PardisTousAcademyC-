using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Courses.Schedules;

/// <summary>
/// Command برای ایجاد زمان‌بندی جدید برای دوره
/// </summary>
public class CreateScheduleCommand : IRequest<OperationResult<Guid>>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public string? Description { get; set; }
}
