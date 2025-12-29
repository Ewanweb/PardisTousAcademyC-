namespace Pardis.Domain.Settings;

/// <summary>
/// تنظیمات پرداخت سیستم
/// </summary>
public class PaymentSettings : BaseEntity
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public PaymentSettings() { }

    public PaymentSettings(string cardNumber, string cardHolderName, string bankName, string? description = null)
    {
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        BankName = bankName;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCardInfo(string cardNumber, string cardHolderName, string bankName, string? description = null)
    {
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        BankName = bankName;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}