using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Consultation;

public class CreateConsultationRequestCommand : IRequest<OperationResult<Guid>>
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
}
