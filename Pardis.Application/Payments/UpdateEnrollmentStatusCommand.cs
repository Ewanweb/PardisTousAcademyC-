using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;

namespace Pardis.Application.Payments;

public class UpdateEnrollmentStatusCommand : IRequest<CourseEnrollmentDto>
{
    public Guid EnrollmentId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public string? Reason { get; set; }
}