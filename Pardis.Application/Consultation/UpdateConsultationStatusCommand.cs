using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Consultation;

namespace Pardis.Application.Consultation;

public class UpdateConsultationStatusCommand : IRequest<OperationResult>
{
    public Guid Id { get; set; }
    public ConsultationStatus Status { get; set; }
    public string? AdminNotes { get; set; }
}
