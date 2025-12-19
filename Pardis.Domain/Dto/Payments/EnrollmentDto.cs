using Pardis.Domain.Payments;

namespace Pardis.Domain.Dto.Payments;

/// <summary>
/// DTO ثبت‌نام دوره
/// </summary>
public class CourseEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public long PaidAmount { get; set; }
    public long RemainingAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusDisplay { get; set; } = string.Empty;
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public string EnrollmentStatusDisplay { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public bool IsInstallmentAllowed { get; set; }
    public int? InstallmentCount { get; set; }
    public decimal PaymentPercentage { get; set; }
    public string? Notes { get; set; }
    public List<InstallmentPaymentDto> Installments { get; set; } = new();
}

/// <summary>
/// DTO ایجاد ثبت‌نام
/// </summary>
public class CreateEnrollmentDto
{
    public Guid CourseId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public bool IsInstallmentAllowed { get; set; }
    public int? InstallmentCount { get; set; }
}

/// <summary>
/// DTO پرداخت قسط
/// </summary>
public class InstallmentPaymentDto
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public long Amount { get; set; }
    public long PaidAmount { get; set; }
    public long RemainingAmount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public InstallmentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public EnrollmentPaymentMethod? PaymentMethod { get; set; }
    public string? PaymentMethodDisplay { get; set; }
    public int DaysUntilDue { get; set; }
    public int DaysOverdue { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO ثبت پرداخت
/// </summary>
public class RecordPaymentDto
{
    public long Amount { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public EnrollmentPaymentMethod Method { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO پروفایل مالی دانشجو
/// </summary>
public class StudentFinancialProfileDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public long TotalEnrollmentAmount { get; set; }
    public long TotalPaidAmount { get; set; }
    public long TotalRemainingAmount { get; set; }
    public decimal OverallPaymentPercentage { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public int OverdueInstallments { get; set; }
    public List<CourseEnrollmentDto> Enrollments { get; set; } = new();
}