using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Accounting;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Query.Accounting;

/// <summary>
/// Handler برای دریافت آمار حسابداری
/// </summary>
public class GetAccountingStatsHandler : IRequestHandler<GetAccountingStatsQuery, AccountingStatsDto>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public GetAccountingStatsHandler(IRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public Task<AccountingStatsDto> Handle(GetAccountingStatsQuery request, CancellationToken cancellationToken)
    {
        // برای الان داده‌های نمونه برمی‌گردانیم تا API کار کند
        // بعداً با داده‌های واقعی جایگزین می‌شود
        
        var result = new AccountingStatsDto
        {
            TotalRevenue = 125000000, // 125 میلیون تومان
            MonthlyRevenue = 15000000, // 15 میلیون تومان
            TotalTransactions = 342,
            ActiveStudents = 156,
            RevenueChange = 12.5m,
            TransactionChange = 8.3m,
            StudentChange = -2.1m,
            MonthlyData = GetSampleMonthlyData(),
            PaymentMethodStats = GetSamplePaymentMethodStats()
        };

        return Task.FromResult(result);
    }

    private List<MonthlyRevenueDto> GetSampleMonthlyData()
    {
        var monthlyData = new List<MonthlyRevenueDto>();
        var random = new Random();
        
        for (int i = 11; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            monthlyData.Add(new MonthlyRevenueDto
            {
                Month = $"{date.Year}/{date.Month:00}",
                Revenue = random.Next(5000000, 20000000), // بین 5 تا 20 میلیون
                TransactionCount = random.Next(10, 50)
            });
        }

        return monthlyData;
    }

    private List<PaymentMethodStatsDto> GetSamplePaymentMethodStats()
    {
        return new List<PaymentMethodStatsDto>
        {
            new PaymentMethodStatsDto
            {
                Method = "Online",
                MethodName = "آنلاین",
                Count = 280,
                Amount = 95000000,
                Percentage = 76.0m
            },
            new PaymentMethodStatsDto
            {
                Method = "Wallet",
                MethodName = "کیف پول",
                Count = 62,
                Amount = 30000000,
                Percentage = 24.0m
            }
        };
    }

    private static string GetPaymentMethodName(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Online => "آنلاین",
            PaymentMethod.Wallet => "کیف پول",
            PaymentMethod.Cash => "نقدی",
            PaymentMethod.Transfer => "انتقال بانکی",
            _ => method.ToString()
        };
    }

    private static decimal CalculatePercentageChange(long current, long previous)
    {
        if (previous == 0)
            return current > 0 ? 100 : 0;

        return (decimal)(current - previous) / previous * 100;
    }
}