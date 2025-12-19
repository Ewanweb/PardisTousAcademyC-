using Pardis.Domain.Courses;
using Pardis.Domain.Users;

namespace Pardis.Domain.Comments;

/// <summary>
/// موجودیت کامنت دوره
/// </summary>
public class CourseComment : BaseEntity
{
    public Guid CourseId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; } // 1-5
    public CommentStatus Status { get; private set; }
    public string? AdminNote { get; private set; } // یادداشت ادمین برای رد/تأیید
    public string? ReviewedByUserId { get; private set; } // ادمینی که بررسی کرده
    public DateTime? ReviewedAt { get; private set; }

    // Navigation Properties
    public Course Course { get; private set; } = null!;
    public User User { get; private set; } = null!;
    public User? ReviewedByUser { get; private set; }

    // Private constructor for EF Core
    private CourseComment() { }

    public CourseComment(Guid courseId, string userId, string content, int rating)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("محتوای کامنت نمی‌تواند خالی باشد", nameof(content));
        
        if (rating < 1 || rating > 5)
            throw new ArgumentException("امتیاز باید بین 1 تا 5 باشد", nameof(rating));

        CourseId = courseId;
        UserId = userId;
        Content = content.Trim();
        Rating = rating;
        Status = CommentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string adminUserId, string? note = null)
    {
        Status = CommentStatus.Approved;
        AdminNote = note;
        ReviewedByUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string adminUserId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("دلیل رد کامنت باید مشخص شود", nameof(reason));

        Status = CommentStatus.Rejected;
        AdminNote = reason;
        ReviewedByUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string newContent, int newRating)
    {
        if (Status != CommentStatus.Pending)
            throw new InvalidOperationException("فقط کامنت‌های در انتظار تأیید قابل ویرایش هستند");

        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("محتوای کامنت نمی‌تواند خالی باشد", nameof(newContent));
        
        if (newRating < 1 || newRating > 5)
            throw new ArgumentException("امتیاز باید بین 1 تا 5 باشد", nameof(newRating));

        Content = newContent.Trim();
        Rating = newRating;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// وضعیت کامنت
/// </summary>
public enum CommentStatus
{
    Pending = 0,    // در انتظار تأیید
    Approved = 1,   // تأیید شده
    Rejected = 2    // رد شده
}