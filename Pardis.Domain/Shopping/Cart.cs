using Pardis.Domain.Courses;
using Pardis.Domain.Users;

namespace Pardis.Domain.Shopping;

/// <summary>
/// موجودیت سبد خرید
/// </summary>
public class Cart : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public long TotalAmount { get; private set; } // مبلغ کل به ریال
    public string Currency { get; private set; } = "IRR";
    public DateTime? ExpiresAt { get; private set; } // تاریخ انقضای سبد

    // Navigation Properties
    public User User { get; private set; } = null!;
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    // Private constructor for EF Core
    private Cart() { }

    public Cart(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("شناسه کاربر نمی‌تواند خالی باشد", nameof(userId));

        UserId = userId;
        TotalAmount = 0;
        Currency = "IRR";
        ExpiresAt = DateTime.UtcNow.AddDays(7); // سبد خرید 7 روز معتبر است
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCourse(Course course)
    {
        if (course == null)
            throw new ArgumentNullException(nameof(course));

        // بررسی اینکه دوره قبلاً در سبد وجود دارد یا نه
        var existingItem = Items.FirstOrDefault(i => i.CourseId == course.Id);
        if (existingItem != null)
            throw new InvalidOperationException("این دوره قبلاً به سبد خرید اضافه شده است");

        var cartItem = new CartItem(Id, course.Id, course.Price, course.Title);
        Items.Add(cartItem);
        
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCourse(Guid courseId)
    {
        var item = Items.FirstOrDefault(i => i.CourseId == courseId);
        if (item == null)
            throw new InvalidOperationException("دوره در سبد خرید یافت نشد");

        Items.Remove(item);
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Clear()
    {
        Items.Clear();
        TotalAmount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmpty() => !Items.Any();

    public bool IsExpired() => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    public void ExtendExpiry(int days = 7)
    {
        ExpiresAt = DateTime.UtcNow.AddDays(days);
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetItemCount() => Items.Count;

    public bool ContainsCourse(Guid courseId) => Items.Any(i => i.CourseId == courseId);

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.UnitPrice);
    }
}