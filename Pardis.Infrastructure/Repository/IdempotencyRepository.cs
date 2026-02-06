using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Idempotency;

namespace Pardis.Infrastructure.Repository;

public class IdempotencyRepository : Repository<IdempotencyRecord>, IIdempotencyRepository
{
    private readonly AppDbContext _context;

    public IdempotencyRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IdempotencyRecord?> GetByKeyAsync(
        string idempotencyKey,
        string userId,
        string operationType,
        CancellationToken cancellationToken = default)
    {
        return await _context.IdempotencyRecords
            .FirstOrDefaultAsync(
                r => r.IdempotencyKey == idempotencyKey &&
                     r.UserId == userId &&
                     r.OperationType == operationType,
                cancellationToken);
    }

    public async Task CleanupExpiredAsync(CancellationToken cancellationToken = default)
    {
        var expiredRecords = await _context.IdempotencyRecords
            .Where(r => r.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _context.IdempotencyRecords.RemoveRange(expiredRecords);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
