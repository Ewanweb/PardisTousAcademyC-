using MediatR;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Application.Payments;

public class CreateEnrollmentCommand : IRequest<CourseEnrollmentDto>
{
    public Guid CourseId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public bool IsInstallmentAllowed { get; set; }
    public int? InstallmentCount { get; set; }
    public string? Notes { get; set; }
}