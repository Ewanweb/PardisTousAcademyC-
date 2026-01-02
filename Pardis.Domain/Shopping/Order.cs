using Pardis.Domain.Users;

namespace Pardis.Domain.Shopping;

/// <summary>
/// موجودیت سفارش (Checkout)
/// </summary>
public class Order : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string OrderNumber { get; private set; } = string.Empty; // شماره سفارش
    public long TotalAmount { get; private set; } // مبلغ کل
    public string Currency { get; private set; } = "IRR";
    public OrderStatus Status { get; private set; }
    public string CartSnapshot { get; private set; } = string.Empty; // JSON snapshot of cart items
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public User User { get; private set; } = null!;
    public ICollection<PaymentAttempt> PaymentAttempts { get; private set; } = new List<PaymentAttempt>();

    // Private constructor for EF Core
    private Order() { }

    public Order(string userId, Cart cart)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("شناسه کاربر نمی‌تواند خالی باشد", nameof(userId));

        if (cart == null)
            throw new ArgumentNullException(nameof(cart));

        if (cart.IsEmpty())
            throw new InvalidOperationException("سبد خرید خالی است");

        UserId = userId;
        OrderNumber = GenerateOrderNumber();
        TotalAmount = cart.TotalAmount;
        Currency = cart.Currency;
        Status = OrderStatus.Draft;
        CartSnapshot = SerializeCartSnapshot(cart);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public PaymentAttempt CreatePaymentAttempt(PaymentMethod method, long amount)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("سفارش قبلاً تکمیل شده است");

        if (amount <= 0)
            throw new ArgumentException("مبلغ پرداخت باید مثبت باشد", nameof(amount));

        var attempt = new PaymentAttempt(Id, UserId, method, amount);
        PaymentAttempts.Add(attempt);
        
        if (Status == OrderStatus.Draft)
        {
            Status = OrderStatus.PendingPayment;
        }

        UpdatedAt = DateTime.UtcNow;
        return attempt;
    }

    public void CompleteOrder()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("سفارش قبلاً تکمیل شده است");

        // بررسی اینکه حداقل یک پرداخت موفق وجود دارد
        var successfulPayment = PaymentAttempts.Any(p => p.Status == PaymentAttemptStatus.Paid);
        if (!successfulPayment)
            throw new InvalidOperationException("هیچ پرداخت موفقی برای این سفارش وجود ندارد");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelOrder(string reason)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("سفارش تکمیل شده قابل لغو نیست");

        Status = OrderStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasSuccessfulPayment() => PaymentAttempts.Any(p => p.Status == PaymentAttemptStatus.Paid);

    public long GetPaidAmount() => PaymentAttempts.Where(p => p.Status == PaymentAttemptStatus.Paid).Sum(p => p.Amount);

    public long GetRemainingAmount() => TotalAmount - GetPaidAmount();

    private string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }

    private string SerializeCartSnapshot(Cart cart)
    {
        // Simple JSON serialization of cart items
        var items = cart.Items.Select(i => new
        {
            CourseId = i.CourseId,
            Title = i.TitleSnapshot,
            Price = i.UnitPrice,
            Thumbnail = i.ThumbnailSnapshot,
            Instructor = i.InstructorSnapshot
        });

        return System.Text.Json.JsonSerializer.Serialize(items);
    }
}

/// <summary>
/// وضعیت سفارش
/// </summary>
public enum OrderStatus
{
    Draft = 0,          // پیش‌نویس
    PendingPayment = 1, // در انتظار پرداخت
    Completed = 2,      // تکمیل شده
    Cancelled = 3       // لغو شده
}

/// <summary>
/// روش پرداخت برای سفارش
/// </summary>
public enum PaymentMethod
{
    Online = 0,     // آنلاین (درگاه)
    Wallet = 1,     // کیف پول
    Manual = 2,     // دستی (کارت به کارت)
    Cash = 3,       // نقدی
    Free = 4        // رایگان
}