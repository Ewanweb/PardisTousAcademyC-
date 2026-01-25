namespace Pardis.Application.Shopping.Cart.AddCourseToCart;

/// <summary>
/// نتیجه اضافه کردن دوره به سبد خرید - بهبود یافته
/// </summary>
public class AddCourseToCartResult
{
    /// <summary>
    /// شناسه سبد خرید
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// شناسه آیتم اضافه شده به سبد
    /// </summary>
    public Guid CartItemId { get; set; }

    /// <summary>
    /// تعداد کل آیتم‌های سبد
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// مبلغ کل سبد خرید
    /// </summary>
    public long TotalAmount { get; set; }

    /// <summary>
    /// عنوان دوره اضافه شده
    /// </summary>
    public string CourseTitle { get; set; } = string.Empty;

    /// <summary>
    /// قیمت دوره اضافه شده
    /// </summary>
    public long CoursePrice { get; set; }

    /// <summary>
    /// پیام موفقیت
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// هشدارها (مثل تغییر قیمت)
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// آیا سبد جدید ایجاد شده است؟
    /// </summary>
    public bool IsNewCart { get; set; }

    /// <summary>
    /// تاریخ انقضای سبد
    /// </summary>
    public DateTime? CartExpiresAt { get; set; }
}