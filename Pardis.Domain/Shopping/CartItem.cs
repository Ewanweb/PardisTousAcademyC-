using Pardis.Domain.Courses;

namespace Pardis.Domain.Shopping;

/// <summary>
/// موجودیت آیتم سبد خرید
/// </summary>
public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid CourseId { get; private set; }
    public long UnitPrice { get; private set; } // قیمت واحد به ریال
    public string TitleSnapshot { get; private set; } = string.Empty; // عنوان دوره در زمان اضافه کردن
    public string? ThumbnailSnapshot { get; private set; } // تصویر دوره در زمان اضافه کردن
    public string? InstructorSnapshot { get; private set; } // نام مدرس در زمان اضافه کردن

    // Navigation Properties
    public Cart Cart { get; private set; } = null!;
    public Course Course { get; private set; } = null!;

    // Private constructor for EF Core
    private CartItem() { }

    public CartItem(Guid cartId, Guid courseId, long unitPrice, string titleSnapshot, 
                   string? thumbnailSnapshot = null, string? instructorSnapshot = null)
    {
        if (unitPrice < 0)
            throw new ArgumentException("قیمت نمی‌تواند منفی باشد", nameof(unitPrice));

        if (string.IsNullOrEmpty(titleSnapshot))
            throw new ArgumentException("عنوان دوره نمی‌تواند خالی باشد", nameof(titleSnapshot));

        CartId = cartId;
        CourseId = courseId;
        UnitPrice = unitPrice;
        TitleSnapshot = titleSnapshot;
        ThumbnailSnapshot = thumbnailSnapshot;
        InstructorSnapshot = instructorSnapshot;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSnapshot(string title, string? thumbnail = null, string? instructor = null)
    {
        TitleSnapshot = title;
        ThumbnailSnapshot = thumbnail;
        InstructorSnapshot = instructor;
        UpdatedAt = DateTime.UtcNow;
    }
}