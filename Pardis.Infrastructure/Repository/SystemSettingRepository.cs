using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Settings;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای تنظیمات سیستم
/// </summary>
public class SystemSettingRepository : ISystemSettingRepository
{
    private readonly AppDbContext _context;

    public SystemSettingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        return setting?.Value;
    }

    public async Task<List<SystemSetting>> GetPublicSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings
            .Where(s => s.IsPublic)
            .OrderBy(s => s.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, string>> GetSettingsByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var keysList = keys.ToList();
        var settings = await _context.SystemSettings
            .Where(s => keysList.Contains(s.Key))
            .ToListAsync(cancellationToken);

        return settings.ToDictionary(s => s.Key, s => s.Value);
    }

    public async Task<SystemSetting> UpsertAsync(string key, string value, string? description = null, bool isPublic = false, CancellationToken cancellationToken = default)
    {
        var existingSetting = await GetByKeyAsync(key, cancellationToken);
        
        if (existingSetting != null)
        {
            existingSetting.UpdateValue(value);
            if (description != null)
                existingSetting.UpdateDescription(description);
            existingSetting.SetPublic(isPublic);
            return existingSetting;
        }

        var newSetting = new SystemSetting(key, value, description, isPublic);
        await _context.SystemSettings.AddAsync(newSetting, cancellationToken);
        return newSetting;
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        if (setting == null)
            return false;

        _context.SystemSettings.Remove(setting);
        return true;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}