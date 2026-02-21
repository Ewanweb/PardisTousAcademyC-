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
        ExpiresAt = DateTime.Now.AddDays(30); // سبد خرید 30 روز (یک ماه) معتبر است
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void AddCourse(Course course)
    {
        if (course == null)
            throw new ArgumentNullException(nameof(course));

        // بررسی اینکه دوره قبلاً در سبد وجود دارد یا نه
        var existingItem = Items.FirstOrDefault(i => i.CourseId == course.Id);
        if (existingItem != null)
            throw new InvalidOperationException("این دوره قبلاً به سبد خرید اضافه شده است");

        // اگر سبد هنوز ذخیره نشده، نمی‌توانیم آیتم اضافه کنیم
        if (Id == Guid.Empty)
            throw new InvalidOperationException("سبد خرید باید قبل از اضافه کردن آیتم ذخیره شود");

        // ایجاد CartItem با اطلاعات کامل دوره
        var thumbnailSnapshot = !string.IsNullOrEmpty(course.Thumbnail) 
            ? course.Thumbnail 
            : "/images/default-course-thumbnail.jpg"; // Default thumbnail fallback
            
        var instructorSnapshot = course.Instructor?.FullName ?? "نامشخص";

        var cartItem = new CartItem(Id, course.Id, course.Price, course.Title, 
                                   thumbnailSnapshot, instructorSnapshot);
        Items.Add(cartItem);
        
        RecalculateTotal();
        
        // تمدید تاریخ انقضای سبد خرید به 30 روز از الان
        ExtendExpiry(30);
        
        UpdatedAt = DateTime.Now;
    }

    public void RemoveCourse(Guid courseId)
    {
        var item = Items.FirstOrDefault(i => i.CourseId == courseId);
        if (item == null)
            throw new InvalidOperationException("دوره در سبد خرید یافت نشد");

        Items.Remove(item);
        RecalculateTotal();
        UpdatedAt = DateTime.Now;
    }

    public void Clear()
    {
        Items.Clear();
        TotalAmount = 0;
        UpdatedAt = DateTime.Now;
    }

    public bool IsEmpty() => !Items.Any();

    public bool IsExpired() => ExpiresAt.HasValue && DateTime.Now > ExpiresAt.Value;

    public void ExtendExpiry(int days = 30)
    {
        ExpiresAt = DateTime.Now.AddDays(days);
        UpdatedAt = DateTime.Now;
    }

    public int GetItemCount() => Items.Count;

    public bool ContainsCourse(Guid courseId) => Items.Any(i => i.CourseId == courseId);

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.UnitPrice);
    }
}