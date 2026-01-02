using MediatR;

namespace Pardis.Query.Settings;

/// <summary>
/// Query برای دریافت اطلاعات پرداخت دستی
/// </summary>
public class GetManualPaymentInfoQuery : IRequest<ManualPaymentInfoDto>
{
}

/// <summary>
/// DTO اطلاعات پرداخت دستی
/// </summary>
public class ManualPaymentInfoDto
{
    /// <summary>
    /// شماره کارت
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// نام صاحب کارت
    /// </summary>
    public string CardHolder { get; set; } = string.Empty;
    
    /// <summary>
    /// نام بانک
    /// </summary>
    public string BankName { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string Description { get; set; } = string.Empty;
}