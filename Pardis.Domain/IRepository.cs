using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Pardis.Domain;

public interface IRepository<T> where T : class 
{

    Task<T?> GetByIdAsync(object id);
    IDbContextTransaction BeginTransaction();
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    Task UpdateAsync(T entity);
    void Remove(T entity);
    Task DeleteAsync(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> SaveChangesAsync(CancellationToken cancellation);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);

    IQueryable<T> Table { get; }
    Task<int> CountAsync();
    Task<List<T>> GetLatestAsync(int count);
    Task<int> CountByDateAsync(int month, int year);

    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}