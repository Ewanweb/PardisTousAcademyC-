using Microsoft.EntityFrameworkCore;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Shopping;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی مخزن سبد خرید
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Course)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Course)
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
    }

    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Carts.AddAsync(cart, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Update(cart);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Remove(cart);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts.AnyAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<List<Cart>> GetExpiredCartsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Carts
            .Where(c => c.ExpiresAt.HasValue && c.ExpiresAt.Value < now)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}