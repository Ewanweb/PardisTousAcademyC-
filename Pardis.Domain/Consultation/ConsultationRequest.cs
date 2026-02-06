namespace Pardis.Domain.Consultation;

public class ConsultationRequest
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }
    public string Message { get; set; } = string.Empty;
    public ConsultationStatus Status { get; set; } = ConsultationStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? ContactedAt { get; set; }
    public string? AdminNotes { get; set; }
    public Guid? UserId { get; set; }
}

public enum ConsultationStatus
{
    Pending = 0,
    Contacted = 1,
    Completed = 2,
    Cancelled = 3
}
