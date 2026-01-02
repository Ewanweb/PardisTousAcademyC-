using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.Contracts;

/// <summary>
/// رابط مخزن سبد خرید
/// </summary>
public interface ICartRepository
{
    Task<Domain.Shopping.Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Domain.Shopping.Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<Domain.Shopping.Cart> CreateAsync(Domain.Shopping.Cart cart, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Shopping.Cart cart, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Shopping.Cart cart, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Domain.Shopping.Cart>> GetExpiredCartsAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}