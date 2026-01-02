using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Settings.UpdateSystemSettings;

/// <summary>
/// Command for updating system settings
/// </summary>
public class UpdateSystemSettingsCommand : IRequest<OperationResult<SystemSettingsResponseDto>>
{
    public int Version { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonIgnore]
    public string? CurrentUserId { get; set; }
}

/// <summary>
/// System settings response DTO
/// </summary>
public class SystemSettingsResponseDto
{
    public int Version { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}