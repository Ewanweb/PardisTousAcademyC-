using MediatR;

namespace Pardis.Application.Attendance;

/// <summary>
/// Command برای حذف جلسه
/// </summary>
public class DeleteSessionCommand : IRequest<bool>
{
    public Guid SessionId { get; set; }
}