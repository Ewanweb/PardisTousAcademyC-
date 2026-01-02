using MediatR;
using Pardis.Domain.Settings;

namespace Pardis.Query.Settings;

/// <summary>
/// Handler برای دریافت اطلاعات پرداخت دستی
/// </summary>
public class GetManualPaymentInfoHandler : IRequestHandler<GetManualPaymentInfoQuery, ManualPaymentInfoDto>
{
    private readonly ISystemSettingRepository _systemSettingRepository;

    public GetManualPaymentInfoHandler(ISystemSettingRepository systemSettingRepository)
    {
        _systemSettingRepository = systemSettingRepository;
    }

    public async Task<ManualPaymentInfoDto> Handle(GetManualPaymentInfoQuery request, CancellationToken cancellationToken)
    {
        var keys = new[]
        {
            SystemSettingKeys.ManualPaymentCardNumber,
            SystemSettingKeys.ManualPaymentCardHolder,
            SystemSettingKeys.ManualPaymentBankName,
            SystemSettingKeys.ManualPaymentDescription
        };

        var settings = await _systemSettingRepository.GetSettingsByKeysAsync(keys, cancellationToken);

        return new ManualPaymentInfoDto
        {
            CardNumber = settings.GetValueOrDefault(SystemSettingKeys.ManualPaymentCardNumber, "6037-9977-****-****"),
            CardHolder = settings.GetValueOrDefault(SystemSettingKeys.ManualPaymentCardHolder, "آکادمی پردیس توس"),
            BankName = settings.GetValueOrDefault(SystemSettingKeys.ManualPaymentBankName, "بانک پاسارگاد"),
            Description = settings.GetValueOrDefault(SystemSettingKeys.ManualPaymentDescription, "لطفاً پس از واریز، رسید پرداخت را آپلود کنید")
        };
    }
}