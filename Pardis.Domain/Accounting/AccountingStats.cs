namespace Pardis.Domain.Accounting;

/// <summary>
/// آمار حسابداری
/// </summary>
public class AccountingStats
{
    public long TotalRevenue { get; set; } // کل درآمد
    public long MonthlyRevenue { get; set; } // درآمد ماهانه
    public int TotalTransactions { get; set; } // کل تراکنش‌ها
    public int ActiveStudents { get; set; } // دانشجویان فعال
    public decimal RevenueChange { get; set; } // تغییر درآمد (درصد)
    public decimal TransactionChange { get; set; } // تغییر تراکنش‌ها
    public decimal StudentChange { get; set; } // تغییر دانشجویان
    public List<MonthlyRevenueData> MonthlyData { get; set; } = new();
    public List<PaymentMethodStats> PaymentMethodStats { get; set; } = new();
}

/// <summary>
/// داده‌های درآمد ماهانه
/// </summary>
public class MonthlyRevenueData
{
    public string Month { get; set; } = string.Empty; // نام ماه
    public long Revenue { get; set; } // درآمد
    public int TransactionCount { get; set; } // تعداد تراکنش
}

/// <summary>
/// آمار روش‌های پرداخت
/// </summary>
public class PaymentMethodStats
{
    public PaymentMethod Method { get; set; } // روش پرداخت
    public string MethodName { get; set; } = string.Empty; // نام روش
    public int Count { get; set; } // تعداد
    public long Amount { get; set; } // مبلغ کل
    public decimal Percentage { get; set; } // درصد
}