namespace Pardis.Domain.Consultation;

public interface IConsultationRequestRepository
{
    Task<ConsultationRequest?> GetByIdAsync(Guid id);
    Task<List<ConsultationRequest>> GetAllAsync();
    Task<List<ConsultationRequest>> GetByStatusAsync(ConsultationStatus status);
    Task<List<ConsultationRequest>> GetByUserIdAsync(Guid userId);
    Task<ConsultationRequest> AddAsync(ConsultationRequest request);
    Task UpdateAsync(ConsultationRequest request);
    Task DeleteAsync(Guid id);
}
