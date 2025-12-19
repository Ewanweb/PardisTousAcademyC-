using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Accounting;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// Repository برای مدیریت تراکنش‌ها
/// </summary>
public class TransactionRepository : Repository<Transaction>
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// دریافت تراکنش با جزئیات کامل
    /// </summary>
    public async Task<Transaction?> GetTransactionWithDetailsAsync(Guid id)
    {
        return await _context.Transactions
            .Include(t => t.User)
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// دریافت تراکنش بر اساس شناسه تراکنش
    /// </summary>
    public async Task<Transaction?> GetByTransactionIdAsync(string transactionId)
    {
        return await _context.Transactions
            .Include(t => t.User)
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    /// <summary>
    /// دریافت تراکنش‌های یک کاربر
    /// </summary>
    public async Task<List<Transaction>> GetUserTransactionsAsync(string userId)
    {
        return await _context.Transactions
            .Include(t => t.Course)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// دریافت تراکنش‌های یک دوره
    /// </summary>
    public async Task<List<Transaction>> GetCourseTransactionsAsync(Guid courseId)
    {
        return await _context.Transactions
            .Include(t => t.User)
            .Where(t => t.CourseId == courseId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// دریافت آمار تراکنش‌ها
    /// </summary>
    public async Task<(long TotalRevenue, int TotalCount)> GetTransactionStatsAsync(
        DateTime? fromDate = null, 
        DateTime? toDate = null,
        TransactionStatus? status = null)
    {
        var query = _context.Transactions.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.CreatedAt <= toDate.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        var totalRevenue = await query.SumAsync(t => t.Amount);
        var totalCount = await query.CountAsync();

        return (totalRevenue, totalCount);
    }

    /// <summary>
    /// دریافت آمار روش‌های پرداخت
    /// </summary>
    public async Task<List<(PaymentMethod Method, int Count, long Amount)>> GetPaymentMethodStatsAsync(
        DateTime? fromDate = null, 
        DateTime? toDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.Status == TransactionStatus.Completed);

        if (fromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.CreatedAt <= toDate.Value);

        return await query
            .GroupBy(t => t.Method)
            .Select(g => new ValueTuple<PaymentMethod, int, long>(
                g.Key,
                g.Count(),
                g.Sum(t => t.Amount)
            ))
            .ToListAsync();
    }

    /// <summary>
    /// دریافت آمار درآمد ماهانه
    /// </summary>
    public async Task<List<(string Month, long Revenue, int Count)>> GetMonthlyRevenueAsync(int months = 12)
    {
        var fromDate = DateTime.Now.AddMonths(-months);

        return await _context.Transactions
            .Where(t => t.Status == TransactionStatus.Completed && t.CreatedAt >= fromDate)
            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new ValueTuple<string, long, int>(
                $"{g.Key.Year}/{g.Key.Month:00}",
                g.Sum(t => t.Amount),
                g.Count()
            ))
            .OrderBy(x => x.Item1)
            .ToListAsync();
    }
}