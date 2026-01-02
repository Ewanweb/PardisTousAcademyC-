using Pardis.Domain.Settings;

namespace Pardis.Domain.Settings;

/// <summary>
/// Repository برای تنظیمات سیستم
/// </summary>
public interface ISystemSettingRepository
{
    /// <summary>
    /// دریافت تنظیم بر اساس کلید
    /// </summary>
    Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت مقدار تنظیم بر اساس کلید
    /// </summary>
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت تمام تنظیمات عمومی
    /// </summary>
    Task<List<SystemSetting>> GetPublicSettingsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت تنظیمات بر اساس کلیدها
    /// </summary>
    Task<Dictionary<string, string>> GetSettingsByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// اضافه کردن یا به‌روزرسانی تنظیم
    /// </summary>
    Task<SystemSetting> UpsertAsync(string key, string value, string? description = null, bool isPublic = false, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// حذف تنظیم
    /// </summary>
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ذخیره تغییرات
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}