using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pardis.Domain;
using System.Linq.Expressions;

namespace Pardis.Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public IQueryable<T> Table => _dbSet;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
    
    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }


    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellation)
    {
        return await _context.SaveChangesAsync(cancellation);
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<List<T>> GetLatestAsync(int count)
    {
        // بررسی می‌کنیم آیا این موجودیت اصلا فیلد CreatedAt دارد یا خیر
        var hasCreatedAt = typeof(T).GetProperty("CreatedAt") != null;

        if (hasCreatedAt)
        {
            // استفاده از EF.Property برای دسترسی داینامیک به فیلد در کوئری SQL
            return await _dbSet
                .OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(count)
                .ToListAsync();
        }

        // اگر CreatedAt نداشت، همینطوری ۱۰ تا برگردان
        return await _dbSet.Take(count).ToListAsync();
    }

    // پیاده‌سازی متد شمارش بر اساس تاریخ بدون اینترفیس
    public async Task<int> CountByDateAsync(int month, int year)
    {
        var hasCreatedAt = typeof(T).GetProperty("CreatedAt") != null;

        if (hasCreatedAt)
        {
            return await _dbSet
                .Where(e => EF.Property<DateTime>(e, "CreatedAt").Month == month &&
                            EF.Property<DateTime>(e, "CreatedAt").Year == year)
                .CountAsync();
        }

        return 0;
    }

    public Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            await operation(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
        });
    }

    public Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            var result = await operation(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
            return result;
        });
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return await _dbSet.AnyAsync(predicate, token);
    }
}