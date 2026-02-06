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
    
    // ==================== Footer Settings ====================
    
    /// <summary>
    /// نام برند در فوتر
    /// </summary>
    public const string FooterBrandName = "Footer.BrandName";
    
    /// <summary>
    /// توضیحات برند در فوتر
    /// </summary>
    public const string FooterBrandDescription = "Footer.BrandDescription";
    
    /// <summary>
    /// آدرس دفتر مرکزی
    /// </summary>
    public const string FooterAddress = "Footer.Address";
    
    /// <summary>
    /// شماره تلفن پشتیبانی
    /// </summary>
    public const string FooterPhone = "Footer.Phone";
    
    /// <summary>
    /// ایمیل تماس
    /// </summary>
    public const string FooterEmail = "Footer.Email";
    
    /// <summary>
    /// لینک اینستاگرام
    /// </summary>
    public const string FooterInstagramUrl = "Footer.InstagramUrl";
    
    /// <summary>
    /// لینک توییتر
    /// </summary>
    public const string FooterTwitterUrl = "Footer.TwitterUrl";
    
    /// <summary>
    /// لینک لینکدین
    /// </summary>
    public const string FooterLinkedinUrl = "Footer.LinkedinUrl";
    
    /// <summary>
    /// لینک یوتیوب
    /// </summary>
    public const string FooterYoutubeUrl = "Footer.YoutubeUrl";
    
    /// <summary>
    /// تعداد دوره‌های آموزشی (آمار)
    /// </summary>
    public const string FooterStatsCoursesCount = "Footer.Stats.CoursesCount";
    
    /// <summary>
    /// تعداد دانشجویان (آمار)
    /// </summary>
    public const string FooterStatsStudentsCount = "Footer.Stats.StudentsCount";
    
    /// <summary>
    /// تعداد مدرسین (آمار)
    /// </summary>
    public const string FooterStatsInstructorsCount = "Footer.Stats.InstructorsCount";
    
    /// <summary>
    /// متن کپی‌رایت
    /// </summary>
    public const string FooterCopyrightText = "Footer.CopyrightText";
    
    /// <summary>
    /// لینک نماد اعتماد الکترونیک
    /// </summary>
    public const string FooterEnamadUrl = "Footer.EnamadUrl";
    
    /// <summary>
    /// کد نماد اعتماد الکترونیک
    /// </summary>
    public const string FooterEnamadCode = "Footer.EnamadCode";
    
    /// <summary>
    /// آیا نمایش خبرنامه فعال است؟
    /// </summary>
    public const string FooterNewsletterEnabled = "Footer.NewsletterEnabled";
    
    /// <summary>
    /// عنوان بخش خبرنامه
    /// </summary>
    public const string FooterNewsletterTitle = "Footer.NewsletterTitle";
    
    /// <summary>
    /// توضیحات بخش خبرنامه
    /// </summary>
    public const string FooterNewsletterDescription = "Footer.NewsletterDescription";
}