using Microsoft.EntityFrameworkCore;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Shopping;
using Pardis.Infrastructure.Extensions;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی مخزن سفارش
/// </summary>
public class OrderRepository : Repository<Order>,IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .Where(o => o.Id == orderId)
            .GetOrdersWithPaymentAttemptsAsync(cancellationToken);
        return orders.FirstOrDefault();
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .Where(o => o.OrderNumber == orderNumber)
            .GetOrdersWithPaymentAttemptsAsync(cancellationToken);
        return orders.FirstOrDefault();
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .GetOrdersWithPaymentAttemptsAsync(cancellationToken);
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
            .Where(o => o.Status == OrderStatus.PendingPayment)
            .GetOrdersWithPaymentAttemptsAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }
}