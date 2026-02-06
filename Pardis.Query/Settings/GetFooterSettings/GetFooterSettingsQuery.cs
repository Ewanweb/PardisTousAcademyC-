using MediatR;

namespace Pardis.Query.Settings.GetFooterSettings;

/// <summary>
/// Query برای دریافت تنظیمات فوتر
/// </summary>
public class GetFooterSettingsQuery : IRequest<FooterSettingsDto>
{
}
