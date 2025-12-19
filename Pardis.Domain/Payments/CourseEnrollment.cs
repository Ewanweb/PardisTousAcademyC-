using Pardis.Domain.Courses;
using Pardis.Domain.Users;

namespace Pardis.Domain.Payments;

/// <summary>
/// موجودیت ثبت‌نام دوره (شامل اطلاعات پرداخت)
/// </summary>
public class CourseEnrollment : BaseEntity
{
    public Guid CourseId { get; private set; }
    public string StudentId { get; private set; } = string.Empty;
    public long TotalAmount { get; private set; } // مبلغ کل به ریال
    public long PaidAmount { get; private set; } // مبلغ پرداخت شده
    public PaymentStatus PaymentStatus { get; private set; }
    public EnrollmentStatus EnrollmentStatus { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public bool IsInstallmentAllowed { get; private set; } // امکان پرداخت قسطی
    public int? InstallmentCount { get; private set; } // تعداد اقساط
    public string? Notes { get; private set; } // یادداشت‌ها

    // Navigation Properties
    public Course Course { get; private set; } = null!;
    public User Student { get; private set; } = null!;
    public ICollection<InstallmentPayment> InstallmentPayments { get; private set; } = new List<InstallmentPayment>();

    // Private constructor for EF Core
    private CourseEnrollment() { }

    public CourseEnrollment(Guid courseId, string studentId, long totalAmount, bool isInstallmentAllowed = false, int? installmentCount = null)
    {
        if (totalAmount <= 0)
            throw new ArgumentException("مبلغ دوره باید مثبت باشد", nameof(totalAmount));

        if (isInstallmentAllowed && (installmentCount == null || installmentCount <= 1))
            throw new ArgumentException("تعداد اقساط باید بیشتر از 1 باشد", nameof(installmentCount));

        CourseId = courseId;
        StudentId = studentId;
        TotalAmount = totalAmount;
        PaidAmount = 0;
        PaymentStatus = PaymentStatus.Unpaid;
        EnrollmentStatus = EnrollmentStatus.Active;
        EnrollmentDate = DateTime.UtcNow;
        IsInstallmentAllowed = isInstallmentAllowed;
        InstallmentCount = installmentCount;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // اگر پرداخت قسطی مجاز است، اقساط را ایجاد کن
        if (isInstallmentAllowed && installmentCount.HasValue)
        {
            CreateInstallments(installmentCount.Value);
        }
    }

    public void AddPayment(long amount, string paymentReference, EnrollmentPaymentMethod method)
    {
        if (amount <= 0)
            throw new ArgumentException("مبلغ پرداخت باید مثبت باشد", nameof(amount));

        if (PaidAmount + amount > TotalAmount)
            throw new InvalidOperationException("مبلغ پرداخت از مبلغ باقی‌مانده بیشتر است");

        PaidAmount += amount;
        UpdatePaymentStatus();
        UpdatedAt = DateTime.UtcNow;

        // اگر پرداخت قسطی است، قسط مربوطه را به‌روزرسانی کن
        if (IsInstallmentAllowed)
        {
            UpdateInstallmentPayments(amount, paymentReference, method);
        }
    }

    public void CompleteEnrollment()
    {
        if (EnrollmentStatus == EnrollmentStatus.Completed)
            throw new InvalidOperationException("ثبت‌نام قبلاً تکمیل شده است");

        EnrollmentStatus = EnrollmentStatus.Completed;
        CompletionDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelEnrollment(string reason)
    {
        if (EnrollmentStatus == EnrollmentStatus.Cancelled)
            throw new InvalidOperationException("ثبت‌نام قبلاً لغو شده است");

        EnrollmentStatus = EnrollmentStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public long GetRemainingAmount() => TotalAmount - PaidAmount;

    public decimal GetPaymentPercentage() => TotalAmount > 0 ? (decimal)PaidAmount / TotalAmount * 100 : 0;

    private void UpdatePaymentStatus()
    {
        if (PaidAmount == 0)
            PaymentStatus = PaymentStatus.Unpaid;
        else if (PaidAmount >= TotalAmount)
            PaymentStatus = PaymentStatus.Paid;
        else
            PaymentStatus = PaymentStatus.Partial;
    }

    private void CreateInstallments(int count)
    {
        var installmentAmount = TotalAmount / count;
        var remainder = TotalAmount % count;

        for (int i = 1; i <= count; i++)
        {
            var amount = installmentAmount;
            // اضافه کردن باقی‌مانده به قسط آخر
            if (i == count) amount += remainder;

            var dueDate = EnrollmentDate.AddMonths(i - 1);
            var installment = new InstallmentPayment(Id, i, amount, dueDate);
            InstallmentPayments.Add(installment);
        }
    }

    private void UpdateInstallmentPayments(long paidAmount, string paymentReference, EnrollmentPaymentMethod method)
    {
        var unpaidInstallments = InstallmentPayments
            .Where(i => i.Status == InstallmentStatus.Unpaid)
            .OrderBy(i => i.InstallmentNumber)
            .ToList();

        var remainingAmount = paidAmount;

        foreach (var installment in unpaidInstallments)
        {
            if (remainingAmount <= 0) break;

            var paymentForThisInstallment = Math.Min(remainingAmount, installment.Amount - installment.PaidAmount);
            installment.AddPayment(paymentForThisInstallment, paymentReference, method);
            remainingAmount -= paymentForThisInstallment;
        }
    }
}

/// <summary>
/// وضعیت پرداخت
/// </summary>
public enum PaymentStatus
{
    Unpaid = 0,     // پرداخت نشده
    Partial = 1,    // پرداخت جزئی
    Paid = 2        // پرداخت کامل
}

/// <summary>
/// وضعیت ثبت‌نام
/// </summary>
public enum EnrollmentStatus
{
    Active = 0,     // فعال
    Completed = 1,  // تکمیل شده
    Cancelled = 2,  // لغو شده
    Suspended = 3   // تعلیق شده
}

/// <summary>
/// روش پرداخت برای ثبت‌نام
/// </summary>
public enum EnrollmentPaymentMethod
{
    Online = 0,     // آنلاین
    Cash = 1,       // نقدی
    Transfer = 2,   // انتقال بانکی
    Cheque = 3      // چک
}