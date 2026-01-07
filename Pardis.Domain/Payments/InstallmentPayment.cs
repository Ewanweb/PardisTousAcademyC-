namespace Pardis.Domain.Payments;

/// <summary>
/// موجودیت پرداخت قسطی
/// </summary>
public class InstallmentPayment : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public int InstallmentNumber { get; private set; } // شماره قسط
    public long Amount { get; private set; } // مبلغ قسط
    public long PaidAmount { get; private set; } // مبلغ پرداخت شده
    public DateTime DueDate { get; private set; } // تاریخ سررسید
    public DateTime? PaidDate { get; private set; } // تاریخ پرداخت
    public InstallmentStatus Status { get; private set; }
    public string? PaymentReference { get; private set; } // شماره مرجع پرداخت
    public EnrollmentPaymentMethod? PaymentMethod { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public CourseEnrollment Enrollment { get; private set; } = null!;

    // Private constructor for EF Core
    private InstallmentPayment() { }

    public InstallmentPayment(Guid enrollmentId, int installmentNumber, long amount, DateTime dueDate)
    {
        if (amount <= 0)
            throw new ArgumentException("مبلغ قسط باید مثبت باشد", nameof(amount));

        if (installmentNumber <= 0)
            throw new ArgumentException("شماره قسط باید مثبت باشد", nameof(installmentNumber));

        Id = Guid.NewGuid();
        EnrollmentId = enrollmentId;
        InstallmentNumber = installmentNumber;
        Amount = amount;
        PaidAmount = 0;
        DueDate = dueDate;
        Status = InstallmentStatus.Unpaid;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPayment(long paymentAmount, string paymentReference, EnrollmentPaymentMethod method)
    {
        if (paymentAmount <= 0)
            throw new ArgumentException("مبلغ پرداخت باید مثبت باشد", nameof(paymentAmount));

        if (PaidAmount + paymentAmount > Amount)
            throw new InvalidOperationException("مبلغ پرداخت از مبلغ باقی‌مانده قسط بیشتر است");

        PaidAmount += paymentAmount;
        PaymentReference = paymentReference;
        PaymentMethod = method;
        
        if (PaidDate == null)
            PaidDate = DateTime.UtcNow;

        UpdateStatus();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOverdue()
    {
        if (Status == InstallmentStatus.Unpaid && DateTime.UtcNow > DueDate)
        {
            Status = InstallmentStatus.Overdue;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public long GetRemainingAmount() => Amount - PaidAmount;

    public int GetDaysUntilDue() => (DueDate.Date - DateTime.UtcNow.Date).Days;

    public int GetDaysOverdue() => DateTime.UtcNow > DueDate ? (DateTime.UtcNow.Date - DueDate.Date).Days : 0;

    private void UpdateStatus()
    {
        if (PaidAmount >= Amount)
            Status = InstallmentStatus.Paid;
        else if (PaidAmount > 0)
            Status = InstallmentStatus.Partial;
        else if (DateTime.UtcNow > DueDate)
            Status = InstallmentStatus.Overdue;
        else
            Status = InstallmentStatus.Unpaid;
    }
}

/// <summary>
/// وضعیت قسط
/// </summary>
public enum InstallmentStatus
{
    Unpaid = 0,     // پرداخت نشده
    Partial = 1,    // پرداخت جزئی
    Paid = 2,       // پرداخت شده
    Overdue = 3     // معوق
}