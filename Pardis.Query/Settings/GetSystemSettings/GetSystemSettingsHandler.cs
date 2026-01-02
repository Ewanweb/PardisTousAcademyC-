using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Settings;

namespace Pardis.Query.Settings.GetSystemSettings;

/// <summary>
/// Handler for getting system settings
/// </summary>
public class GetSystemSettingsHandler : IRequestHandler<GetSystemSettingsQuery, SystemSettingsDto>
{
    private readonly IRepository<SystemSetting> _settingsRepository;

    public GetSystemSettingsHandler(IRepository<SystemSetting> settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<SystemSettingsDto> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _settingsRepository.Table
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var data = settings.ToDictionary(s => s.Key, s => s.Value);

        // Get the latest update info
        var latestSetting = settings.OrderByDescending(s => s.UpdatedAt).FirstOrDefault();

        return new SystemSettingsDto
        {
            Version = 1, // Simple version for now, could be enhanced with actual versioning
            UpdatedAt = latestSetting?.UpdatedAt ?? DateTime.UtcNow,
            UpdatedBy = "System", // Could be enhanced to track actual user
            Data = data
        };
    }
}