using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Consultation;

namespace Pardis.Infrastructure.Repository;

public class ConsultationRequestRepository : IConsultationRequestRepository
{
    private readonly AppDbContext _context;

    public ConsultationRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultationRequest?> GetByIdAsync(Guid id)
    {
        return await _context.ConsultationRequests.FindAsync(id);
    }

    public async Task<List<ConsultationRequest>> GetAllAsync()
    {
        return await _context.ConsultationRequests
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ConsultationRequest>> GetByStatusAsync(ConsultationStatus status)
    {
        return await _context.ConsultationRequests
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ConsultationRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ConsultationRequests
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<ConsultationRequest> AddAsync(ConsultationRequest request)
    {
        await _context.ConsultationRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task UpdateAsync(ConsultationRequest request)
    {
        _context.ConsultationRequests.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var request = await GetByIdAsync(id);
        if (request != null)
        {
            _context.ConsultationRequests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}
