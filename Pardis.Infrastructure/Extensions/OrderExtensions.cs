using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Shopping;

namespace Pardis.Infrastructure.Extensions;

/// <summary>
/// متدهای کمکی برای کار با Order و PaymentAttempt
/// </summary>
public static class OrderExtensions
{
    /// <summary>
    /// دریافت لیست سفارش‌ها با PaymentAttempts (بدون نیاز به Safe loading پس از رفع مشکل CartId)
    /// </summary>
    public static async Task<List<Order>> GetOrdersWithPaymentAttemptsAsync(
        this IQueryable<Order> query, 
        CancellationToken cancellationToken = default)
    {
        return await query
            .Include(o => o.PaymentAttempts)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// بررسی اینکه آیا Order دارای PaymentAttempts معتبر است
    /// </summary>
    public static bool HasValidPaymentAttempts(this Order order)
    {
        return order.PaymentAttempts?.Any() == true;
    }

    /// <summary>
    /// دریافت PaymentAttempts از Order
    /// </summary>
    public static ICollection<PaymentAttempt> GetPaymentAttempts(this Order order)
    {
        return order.PaymentAttempts ?? new List<PaymentAttempt>();
    }

    // LEGACY METHODS - Keep for backward compatibility during transition
    // These can be removed after confirming all NULL CartId issues are resolved

    /// <summary>
    /// بارگذاری ایمن PaymentAttempts برای یک Order (LEGACY - for backward compatibility)
    /// </summary>
    [Obsolete("Use standard EF Core Include after CartId NULL fix")]
    public static async Task<Order> SafeLoadPaymentAttemptsAsync(this Order order, AppDbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await context.Entry(order)
                .Collection(o => o.PaymentAttempts)
                .LoadAsync(cancellationToken);
        }
        catch (Exception)
        {
            // If any error occurs, clear PaymentAttempts
            order.PaymentAttempts.Clear();
        }

        return order;
    }

    /// <summary>
    /// دریافت ایمن لیست سفارش‌ها با PaymentAttempts (LEGACY - for backward compatibility)
    /// </summary>
    [Obsolete("Use GetOrdersWithPaymentAttemptsAsync after CartId NULL fix")]
    public static async Task<List<Order>> SafeGetOrdersWithPaymentAttemptsAsync(
        this IQueryable<Order> query, 
        AppDbContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query
                .Include(o => o.PaymentAttempts)
                .ToListAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Fallback: load without PaymentAttempts
            var orders = await query.ToListAsync(cancellationToken);
            foreach (var order in orders)
            {
                await order.SafeLoadPaymentAttemptsAsync(context, cancellationToken);
            }
            return orders;
        }
    }

    /// <summary>
    /// دریافت ایمن PaymentAttempts از Order (LEGACY - for backward compatibility)
    /// </summary>
    [Obsolete("Use GetPaymentAttempts after CartId NULL fix")]
    public static ICollection<PaymentAttempt> GetSafePaymentAttempts(this Order order)
    {
        try
        {
            return order.PaymentAttempts ?? new List<PaymentAttempt>();
        }
        catch
        {
            return new List<PaymentAttempt>();
        }
    }
}