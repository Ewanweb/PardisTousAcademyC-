using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Consultation;

namespace Pardis.Application.Consultation;

public class UpdateConsultationStatusHandler : IRequestHandler<UpdateConsultationStatusCommand, OperationResult>
{
    private readonly IConsultationRequestRepository _repository;

    public UpdateConsultationStatusHandler(IConsultationRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult> Handle(UpdateConsultationStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var consultationRequest = await _repository.GetByIdAsync(request.Id);
            
            if (consultationRequest == null)
            {
                return OperationResult.NotFound("درخواست مشاوره یافت نشد");
            }

            consultationRequest.Status = request.Status;
            
            if (!string.IsNullOrWhiteSpace(request.AdminNotes))
            {
                consultationRequest.AdminNotes = request.AdminNotes;
            }

            if (request.Status == ConsultationStatus.Contacted && !consultationRequest.ContactedAt.HasValue)
            {
                consultationRequest.ContactedAt = DateTime.UtcNow;
            }

            await _repository.UpdateAsync(consultationRequest);

            return OperationResult.Success("وضعیت درخواست با موفقیت به‌روزرسانی شد");
        }
        catch (Exception ex)
        {
            return OperationResult.Error($"خطا در به‌روزرسانی وضعیت: {ex.Message}");
        }
    }
}
