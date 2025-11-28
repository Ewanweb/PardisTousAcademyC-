using Microsoft.EntityFrameworkCore.Storage;

namespace Pardis.Domain;

public interface IRepository<T> where T : class 
{

    Task<T?> GetByIdAsync(object id);
    IDbContextTransaction BeginTransaction();
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> SaveChangesAsync(CancellationToken cancellation);


    Task<int> CountAsync();
    Task<List<T>> GetLatestAsync(int count);
    Task<int> CountByDateAsync(int month, int year);


}