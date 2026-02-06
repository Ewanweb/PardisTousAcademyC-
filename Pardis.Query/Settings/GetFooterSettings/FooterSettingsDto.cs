namespace Pardis.Query.Settings.GetFooterSettings;

/// <summary>
/// DTO برای تنظیمات فوتر
/// </summary>
public class FooterSettingsDto
{
    /// <summary>
    /// اطلاعات برند
    /// </summary>
    public BrandInfo Brand { get; set; } = new();
    
    /// <summary>
    /// اطلاعات تماس
    /// </summary>
    public ContactInfo Contact { get; set; } = new();
    
    /// <summary>
    /// لینک‌های شبکه‌های اجتماعی
    /// </summary>
    public SocialMediaLinks SocialMedia { get; set; } = new();
    
    /// <summary>
    /// آمار سایت
    /// </summary>
    public SiteStats Stats { get; set; } = new();
    
    /// <summary>
    /// تنظیمات خبرنامه
    /// </summary>
    public NewsletterSettings Newsletter { get; set; } = new();
    
    /// <summary>
    /// اطلاعات کپی‌رایت
    /// </summary>
    public CopyrightInfo Copyright { get; set; } = new();
    
    /// <summary>
    /// اطلاعات نماد اعتماد
    /// </summary>
    public EnamadInfo Enamad { get; set; } = new();
}

/// <summary>
/// اطلاعات برند
/// </summary>
public class BrandInfo
{
    /// <summary>
    /// نام برند
    /// </summary>
    public string Name { get; set; } = "پردیس توس";
    
    /// <summary>
    /// توضیحات برند
    /// </summary>
    public string Description { get; set; } = "بهترین پلتفرم آموزش آنلاین ایران";
}

/// <summary>
/// اطلاعات تماس
/// </summary>
public class ContactInfo
{
    /// <summary>
    /// آدرس
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// شماره تلفن
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// ایمیل
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// لینک‌های شبکه‌های اجتماعی
/// </summary>
public class SocialMediaLinks
{
    /// <summary>
    /// لینک اینستاگرام
    /// </summary>
    public string? Instagram { get; set; }
    
    /// <summary>
    /// لینک توییتر
    /// </summary>
    public string? Twitter { get; set; }
    
    /// <summary>
    /// لینک لینکدین
    /// </summary>
    public string? Linkedin { get; set; }
    
    /// <summary>
    /// لینک یوتیوب
    /// </summary>
    public string? Youtube { get; set; }
}

/// <summary>
/// آمار سایت
/// </summary>
public class SiteStats
{
    /// <summary>
    /// تعداد دوره‌ها
    /// </summary>
    public string CoursesCount { get; set; } = "۱۰۰۰+";
    
    /// <summary>
    /// تعداد دانشجویان
    /// </summary>
    public string StudentsCount { get; set; } = "۵۰۰۰+";
    
    /// <summary>
    /// تعداد مدرسین
    /// </summary>
    public string InstructorsCount { get; set; } = "۱۰۰+";
}

/// <summary>
/// تنظیمات خبرنامه
/// </summary>
public class NewsletterSettings
{
    /// <summary>
    /// آیا خبرنامه فعال است؟
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// عنوان
    /// </summary>
    public string Title { get; set; } = "عضویت در خبرنامه";
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string Description { get; set; } = "از آخرین دوره‌ها و تخفیف‌های ویژه باخبر شوید";
}

/// <summary>
/// اطلاعات کپی‌رایت
/// </summary>
public class CopyrightInfo
{
    /// <summary>
    /// متن کپی‌رایت
    /// </summary>
    public string Text { get; set; } = "آکادمی پردیس توس - تمامی حقوق محفوظ است";
}

/// <summary>
/// اطلاعات نماد اعتماد
/// </summary>
public class EnamadInfo
{
    /// <summary>
    /// لینک نماد
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// کد نماد
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
