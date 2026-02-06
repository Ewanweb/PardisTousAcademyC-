using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Settings;

namespace Pardis.Application.Settings.UpdateSystemSettings;

/// <summary>
/// Handler for updating system settings
/// </summary>
public class UpdateSystemSettingsHandler : IRequestHandler<UpdateSystemSettingsCommand, OperationResult<SystemSettingsResponseDto>>
{
    private readonly IRepository<SystemSetting> _settingsRepository;

    public UpdateSystemSettingsHandler(IRepository<SystemSetting> settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<OperationResult<SystemSettingsResponseDto>> Handle(UpdateSystemSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing settings
            var existingSettings = await _settingsRepository.Table
                .ToListAsync(cancellationToken);

            // Update existing settings and add new ones
            foreach (var kvp in request.Data)
            {
                var existingSetting = existingSettings.FirstOrDefault(s => s.Key == kvp.Key);
                
                if (existingSetting != null)
                {
                    existingSetting.UpdateValue(kvp.Value);
                    _settingsRepository.Update(existingSetting);
                }
                else
                {
                    var newSetting = new SystemSetting(kvp.Key, kvp.Value);
                    await _settingsRepository.AddAsync(newSetting);
                }
            }
            
            await _settingsRepository.SaveChangesAsync(cancellationToken);

            // Return updated settings
            var updatedSettings = await _settingsRepository.Table
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var data = updatedSettings.ToDictionary(s => s.Key, s => s.Value);
            var latestSetting = updatedSettings.OrderByDescending(s => s.UpdatedAt).FirstOrDefault();

            var result = new SystemSettingsResponseDto
            {
                Version = request.Version + 1, // Increment version
                UpdatedAt = latestSetting?.UpdatedAt ?? DateTime.UtcNow,
                UpdatedBy = request.CurrentUserId ?? "System",
                Data = data
            };

            return OperationResult<SystemSettingsResponseDto>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<SystemSettingsResponseDto>.Error($"Failed to update system settings: {ex.Message}");
        }
    }
}