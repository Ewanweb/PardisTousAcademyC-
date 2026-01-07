namespace Pardis.Domain.Shopping;

/// <summary>
/// روش پرداخت برای سفارش - فقط پرداخت دستی
/// </summary>
public enum PaymentMethod
{
    Manual = 2     // دستی (کارت به کارت) - تنها روش پرداخت مجاز
}