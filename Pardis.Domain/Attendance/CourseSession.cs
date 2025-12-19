using Pardis.Domain.Courses;

namespace Pardis.Domain.Attendance;

/// <summary>
/// موجودیت جلسه دوره
/// </summary>
public class CourseSession : BaseEntity
{
    public Guid CourseId { get; private set; }
    public Guid? ScheduleId { get; private set; } // اضافه شده برای ارتباط با زمان‌بندی
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime SessionDate { get; private set; }
    public TimeSpan Duration { get; private set; } // مدت زمان جلسه
    public int SessionNumber { get; private set; } // شماره جلسه
    public SessionStatus Status { get; private set; }

    // Navigation Properties
    public Course Course { get; private set; } = null!;
    public CourseSchedule? Schedule { get; private set; }
    public ICollection<StudentAttendance> Attendances { get; private set; } = new List<StudentAttendance>();

    // Private constructor for EF Core
    private CourseSession() { }

    public CourseSession(Guid courseId, string title, DateTime sessionDate, TimeSpan duration, int sessionNumber, string? description = null, Guid? scheduleId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("عنوان جلسه نمی‌تواند خالی باشد", nameof(title));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("مدت زمان جلسه باید مثبت باشد", nameof(duration));

        if (sessionNumber <= 0)
            throw new ArgumentException("شماره جلسه باید مثبت باشد", nameof(sessionNumber));

        CourseId = courseId;
        ScheduleId = scheduleId;
        Title = title.Trim();
        Description = description?.Trim();
        SessionDate = sessionDate;
        Duration = duration;
        SessionNumber = sessionNumber;
        Status = SessionStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSession(string title, DateTime sessionDate, TimeSpan duration, string? description = null)
    {
        if (Status == SessionStatus.Completed)
            throw new InvalidOperationException("جلسه تکمیل شده قابل ویرایش نیست");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("عنوان جلسه نمی‌تواند خالی باشد", nameof(title));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("مدت زمان جلسه باید مثبت باشد", nameof(duration));

        Title = title.Trim();
        Description = description?.Trim();
        SessionDate = sessionDate;
        Duration = duration;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartSession()
    {
        if (Status != SessionStatus.Scheduled)
            throw new InvalidOperationException("فقط جلسات زمان‌بندی شده قابل شروع هستند");

        Status = SessionStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteSession()
    {
        if (Status != SessionStatus.InProgress)
            throw new InvalidOperationException("فقط جلسات در حال برگزاری قابل تکمیل هستند");

        Status = SessionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelSession()
    {
        if (Status == SessionStatus.Completed)
            throw new InvalidOperationException("جلسه تکمیل شده قابل لغو نیست");

        Status = SessionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// وضعیت جلسه
/// </summary>
public enum SessionStatus
{
    Scheduled = 0,  // زمان‌بندی شده
    InProgress = 1, // در حال برگزاری
    Completed = 2,  // تکمیل شده
    Cancelled = 3   // لغو شده
}