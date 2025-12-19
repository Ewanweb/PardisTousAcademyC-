using Pardis.Domain.Users;

namespace Pardis.Domain.Attendance;

/// <summary>
/// موجودیت حضور و غیاب دانشجو
/// </summary>
public class StudentAttendance : BaseEntity
{
    public Guid SessionId { get; private set; }
    public string StudentId { get; private set; } = string.Empty;
    public AttendanceStatus Status { get; private set; }
    public DateTime? CheckInTime { get; private set; } // زمان ورود
    public DateTime? CheckOutTime { get; private set; } // زمان خروج
    public string? Note { get; private set; } // یادداشت (مثل دلیل تأخیر)
    public string? RecordedByUserId { get; private set; } // کسی که حضور را ثبت کرده

    // Navigation Properties
    public CourseSession Session { get; private set; } = null!;
    public User Student { get; private set; } = null!;
    public User? RecordedByUser { get; private set; }

    // Private constructor for EF Core
    private StudentAttendance() { }

    public StudentAttendance(Guid sessionId, string studentId, AttendanceStatus status, string recordedByUserId, string? note = null)
    {
        SessionId = sessionId;
        StudentId = studentId;
        Status = status;
        Note = note?.Trim();
        RecordedByUserId = recordedByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        if (status == AttendanceStatus.Present || status == AttendanceStatus.Late)
        {
            CheckInTime = DateTime.UtcNow;
        }
    }

    public static StudentAttendance Create(Guid sessionId, string studentId, AttendanceStatus status, 
        DateTime? checkInTime, DateTime? checkOutTime, string? note, string recordedByUserId)
    {
        var attendance = new StudentAttendance
        {
            SessionId = sessionId,
            StudentId = studentId,
            Status = status,
            CheckInTime = checkInTime,
            CheckOutTime = checkOutTime,
            Note = note?.Trim(),
            RecordedByUserId = recordedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return attendance;
    }

    public void UpdateAttendance(AttendanceStatus newStatus, DateTime? checkInTime, DateTime? checkOutTime, string? note)
    {
        Status = newStatus;
        CheckInTime = checkInTime;
        CheckOutTime = checkOutTime;
        Note = note?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAttendance(AttendanceStatus newStatus, string updatedByUserId, string? note = null)
    {
        Status = newStatus;
        Note = note?.Trim();
        RecordedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;

        // اگر از غایب به حاضر تغییر کرد، زمان ورود را ثبت کن
        if ((Status == AttendanceStatus.Present || Status == AttendanceStatus.Late) && CheckInTime == null)
        {
            CheckInTime = DateTime.UtcNow;
        }
        
        // اگر به غایب تغییر کرد، زمان‌ها را پاک کن
        if (Status == AttendanceStatus.Absent)
        {
            CheckInTime = null;
            CheckOutTime = null;
        }
    }

    public void CheckOut()
    {
        if (Status == AttendanceStatus.Absent)
            throw new InvalidOperationException("دانشجوی غایب نمی‌تواند خروج ثبت کند");

        if (CheckOutTime != null)
            throw new InvalidOperationException("خروج قبلاً ثبت شده است");

        CheckOutTime = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public TimeSpan? GetAttendanceDuration()
    {
        if (CheckInTime == null) return null;
        
        var endTime = CheckOutTime ?? DateTime.UtcNow;
        return endTime - CheckInTime.Value;
    }
}

/// <summary>
/// وضعیت حضور و غیاب
/// </summary>
public enum AttendanceStatus
{
    Present = 0,    // حاضر
    Absent = 1,     // غایب
    Late = 2        // تأخیر
}