using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.Contracts;

/// <summary>
/// رابط مخزن سفارش
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}