using MediatR;

namespace Pardis.Query.Settings.GetSystemSettings;

/// <summary>
/// Query for getting system settings
/// </summary>
public class GetSystemSettingsQuery : IRequest<SystemSettingsDto>
{
}

/// <summary>
/// System settings DTO
/// </summary>
public class SystemSettingsDto
{
    public int Version { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}