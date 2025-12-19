using Pardis.Domain.Comments;

namespace Pardis.Domain.Dto.Comments;

/// <summary>
/// DTO کامنت دوره
/// </summary>
public class CourseCommentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public CommentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
    public string? ReviewedByUserName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO ایجاد کامنت
/// </summary>
public class CreateCommentDto
{
    public Guid CourseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}

/// <summary>
/// DTO بروزرسانی کامنت
/// </summary>
public class UpdateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}

/// <summary>
/// DTO بررسی کامنت توسط ادمین
/// </summary>
public class ReviewCommentDto
{
    public CommentStatus Status { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO آمار کامنت‌های دوره
/// </summary>
public class CourseCommentStatsDto
{
    public Guid CourseId { get; set; }
    public int TotalComments { get; set; }
    public int ApprovedComments { get; set; }
    public int PendingComments { get; set; }
    public int RejectedComments { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
}