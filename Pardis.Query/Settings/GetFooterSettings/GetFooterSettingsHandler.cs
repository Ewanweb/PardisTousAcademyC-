using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Settings;
using Pardis.Infrastructure;

namespace Pardis.Query.Settings.GetFooterSettings;

/// <summary>
/// Handler برای دریافت تنظیمات فوتر
/// </summary>
public class GetFooterSettingsHandler : IRequestHandler<GetFooterSettingsQuery, FooterSettingsDto>
{
    private readonly AppDbContext _context;

    public GetFooterSettingsHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FooterSettingsDto> Handle(GetFooterSettingsQuery request, CancellationToken cancellationToken)
    {
        // دریافت تمام تنظیمات فوتر از دیتابیس
        var settings = await _context.SystemSettings
            .Where(s => s.Key.StartsWith("Footer."))
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Key, s => s.Value, cancellationToken);

        // ساخت DTO با مقادیر پیش‌فرض
        var result = new FooterSettingsDto
        {
            Brand = new BrandInfo
            {
                Name = GetSetting(settings, SystemSettingKeys.FooterBrandName, "پردیس توس"),
                Description = GetSetting(settings, SystemSettingKeys.FooterBrandDescription, 
                    "بهترین پلتفرم آموزش آنلاین ایران با بیش از ۱۰۰۰ دوره تخصصی و اساتید مجرب.")
            },
            Contact = new ContactInfo
            {
                Address = GetSetting(settings, SystemSettingKeys.FooterAddress, "تهران، خیابان ولیعصر، پلاک ۱۲۳"),
                Phone = GetSetting(settings, SystemSettingKeys.FooterPhone, "021-1234-5678"),
                Email = GetSetting(settings, SystemSettingKeys.FooterEmail, "info@pardistous.ir")
            },
            SocialMedia = new SocialMediaLinks
            {
                Instagram = GetSetting(settings, SystemSettingKeys.FooterInstagramUrl, null),
                Twitter = GetSetting(settings, SystemSettingKeys.FooterTwitterUrl, null),
                Linkedin = GetSetting(settings, SystemSettingKeys.FooterLinkedinUrl, null),
                Youtube = GetSetting(settings, SystemSettingKeys.FooterYoutubeUrl, null)
            },
            Stats = new SiteStats
            {
                CoursesCount = GetSetting(settings, SystemSettingKeys.FooterStatsCoursesCount, "۱۰۰۰+"),
                StudentsCount = GetSetting(settings, SystemSettingKeys.FooterStatsStudentsCount, "۵۰۰۰+"),
                InstructorsCount = GetSetting(settings, SystemSettingKeys.FooterStatsInstructorsCount, "۱۰۰+")
            },
            Newsletter = new NewsletterSettings
            {
                Enabled = GetBoolSetting(settings, SystemSettingKeys.FooterNewsletterEnabled, true),
                Title = GetSetting(settings, SystemSettingKeys.FooterNewsletterTitle, "عضویت در خبرنامه"),
                Description = GetSetting(settings, SystemSettingKeys.FooterNewsletterDescription, 
                    "از آخرین دوره‌ها و تخفیف‌های ویژه باخبر شوید")
            },
            Copyright = new CopyrightInfo
            {
                Text = GetSetting(settings, SystemSettingKeys.FooterCopyrightText, 
                    "آکادمی پردیس توس - تمامی حقوق محفوظ است")
            },
            Enamad = new EnamadInfo
            {
                Url = GetSetting(settings, SystemSettingKeys.FooterEnamadUrl, 
                    "https://trustseal.enamad.ir/?id=5272990&Code=fDfKAiPgvcH664AEtkOpBLvv4wGKnNO9"),
                Code = GetSetting(settings, SystemSettingKeys.FooterEnamadCode, "fDfKAiPgvcH664AEtkOpBLvv4wGKnNO9")
            }
        };

        return result;
    }

    /// <summary>
    /// دریافت مقدار تنظیم با مقدار پیش‌فرض
    /// </summary>
    private string GetSetting(Dictionary<string, string> settings, string key, string? defaultValue)
    {
        return settings.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) 
            ? value 
            : defaultValue ?? string.Empty;
    }

    /// <summary>
    /// دریافت مقدار boolean از تنظیمات
    /// </summary>
    private bool GetBoolSetting(Dictionary<string, string> settings, string key, bool defaultValue)
    {
        if (settings.TryGetValue(key, out var value))
        {
            return value.ToLower() == "true" || value == "1";
        }
        return defaultValue;
    }
}
