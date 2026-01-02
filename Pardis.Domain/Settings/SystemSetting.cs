using Pardis.Domain;

namespace Pardis.Domain.Settings;

/// <summary>
/// تنظیمات سیستم
/// </summary>
public class SystemSetting : BaseEntity
{
    /// <summary>
    /// کلید تنظیم
    /// </summary>
    public string Key { get; private set; } = string.Empty;
    
    /// <summary>
    /// مقدار تنظیم
    /// </summary>
    public string Value { get; private set; } = string.Empty;
    
    /// <summary>
    /// توضیحات تنظیم
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// آیا این تنظیم عمومی است؟
    /// </summary>
    public bool IsPublic { get; private set; }

    /// <summary>
    /// سازنده خصوصی برای EF Core
    /// </summary>
    private SystemSetting() { }

    /// <summary>
    /// سازنده برای ایجاد تنظیم جدید
    /// </summary>
    public SystemSetting(string key, string value, string? description = null, bool isPublic = false)
    {
        Key = key;
        Value = value;
        Description = description;
        IsPublic = isPublic;
    }

    /// <summary>
    /// به‌روزرسانی مقدار تنظیم
    /// </summary>
    public void UpdateValue(string value)
    {
        Value = value;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// به‌روزرسانی توضیحات
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تغییر وضعیت عمومی بودن
    /// </summary>
    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// کلیدهای تنظیمات سیستم
/// </summary>
public static class SystemSettingKeys
{
    /// <summary>
    /// شماره کارت مقصد برای پرداخت دستی
    /// </summary>
    public const string ManualPaymentCardNumber = "ManualPayment.CardNumber";
    
    /// <summary>
    /// نام صاحب کارت
    /// </summary>
    public const string ManualPaymentCardHolder = "ManualPayment.CardHolder";
    
    /// <summary>
    /// نام بانک
    /// </summary>
    public const string ManualPaymentBankName = "ManualPayment.BankName";
    
    /// <summary>
    /// توضیحات پرداخت دستی
    /// </summary>
    public const string ManualPaymentDescription = "ManualPayment.Description";
}