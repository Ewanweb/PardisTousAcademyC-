using System.ComponentModel.DataAnnotations;

namespace Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;

/// <summary>
/// درخواست تأیید پرداخت توسط مدیر
/// </summary>
public class ApprovePaymentRequest
{
    /// <summary>
    /// یادداشت مدیر برای تأیید پرداخت
    /// </summary>
    [MaxLength(500)]
    public string? AdminNote { get; set; }

    /// <summary>
    /// آیا پرداخت تأیید شود؟
    /// </summary>
    [Required]
    public bool IsApproved { get; set; }

    /// <summary>
    /// دلیل رد پرداخت (در صورت عدم تأیید)
    /// </summary>
    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    /// <summary>
    /// مبلغ تأیید شده (در صورت تفاوت با مبلغ درخواستی)
    /// </summary>
    public decimal? ApprovedAmount { get; set; }
}