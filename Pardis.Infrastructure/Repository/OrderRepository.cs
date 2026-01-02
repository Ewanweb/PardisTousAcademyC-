using Microsoft.EntityFrameworkCore;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Shopping;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی مخزن سفارش
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.PaymentAttempts)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.PaymentAttempts)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.PaymentAttempts)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Orders.AddAsync(order, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        await Task.CompletedTask;
    }

    public async Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.PaymentAttempts)
            .Where(o => o.Status == OrderStatus.PendingPayment)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}