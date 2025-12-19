using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;

namespace Pardis.Application.Payments;

public class AddEnrollmentPaymentCommand : IRequest<CourseEnrollmentDto>
{
    public Guid EnrollmentId { get; set; }
    public long Amount { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public EnrollmentPaymentMethod Method { get; set; }
    public string? Notes { get; set; }
}