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
        await using var transaction = _settingsRepository.BeginTransaction();
        
        try
        {
            // Get existing settings
            var existingSettings = await _settingsRepository.Table
                .ToListAsync(cancellationToken);

            // Simple version check - in a real system, you'd want proper optimistic concurrency
            var latestUpdate = existingSettings.OrderByDescending(s => s.UpdatedAt).FirstOrDefault()?.UpdatedAt ?? DateTime.MinValue;
            
            // For now, we'll skip strict version checking and just update
            // In production, you'd implement proper optimistic concurrency control

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

            // Remove settings that are no longer in the request (optional behavior)
            // For safety, we'll keep existing settings that aren't in the update
            
            await _settingsRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

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
            await transaction.RollbackAsync(cancellationToken);
            return OperationResult<SystemSettingsResponseDto>.Error($"Failed to update system settings: {ex.Message}");
        }
    }
}