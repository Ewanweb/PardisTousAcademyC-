using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Accounting;
using Pardis.Domain.Shopping;
using Pardis.Domain.Payments;

namespace Pardis.Query.Accounting.GetAccountingSummary;

/// <summary>
/// Handler برای دریافت خلاصه آمار حسابداری - ساده شده برای PaymentAttempt فقط
/// </summary>
public class GetAccountingSummaryHandler : IRequestHandler<GetAccountingSummaryQuery, GetAccountingSummaryResult>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<CourseEnrollment> _enrollmentRepository;
    private readonly IRepository<PaymentAttempt> _paymentAttemptRepository;

    public GetAccountingSummaryHandler(
        IRepository<Transaction> transactionRepository,
        IRepository<Order> orderRepository,
        IRepository<CourseEnrollment> enrollmentRepository,
        IRepository<PaymentAttempt> paymentAttemptRepository)
    {
        _transactionRepository = transactionRepository;
        _orderRepository = orderRepository;
        _enrollmentRepository = enrollmentRepository;
        _paymentAttemptRepository = paymentAttemptRepository;
    }

    public async Task<GetAccountingSummaryResult> Handle(GetAccountingSummaryQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var yearStart = new DateTime(today.Year, 1, 1);

        // محاسبه درآمد
        var revenueStats = await CalculateRevenueStats(today, weekStart, monthStart, yearStart, cancellationToken);
        
        // محاسبه تراکنش‌ها
        var transactionStats = await CalculateTransactionStats(today, weekStart, monthStart, yearStart, cancellationToken);
        
        // محاسبه دانشجویان
        var studentStats = await CalculateStudentStats(today, weekStart, monthStart, cancellationToken);
        
        // محاسبه پرداخت‌های دستی (PaymentAttempts)
        var manualPaymentStats = await CalculateManualPaymentStats(cancellationToken);
        
        // محاسبه آمار کلی
        var generalStats = await CalculateGeneralStats(cancellationToken);
        
        // چارت درآمد روزانه (30 روز گذشته)
        var revenueChart = await GetRevenueChart(today.AddDays(-30), today, cancellationToken);
        
        // آمار روش‌های پرداخت
        var paymentMethodStats = await GetPaymentMethodStats(cancellationToken);

        return new GetAccountingSummaryResult
        {
            TotalRevenue = revenueStats.Total,
            MonthlyRevenue = revenueStats.Monthly,
            WeeklyRevenue = revenueStats.Weekly,
            TodayRevenue = revenueStats.Today,
            
            TotalTransactions = transactionStats.Total,
            MonthlyTransactions = transactionStats.Monthly,
            WeeklyTransactions = transactionStats.Weekly,
            TodayTransactions = transactionStats.Today,
            
            TotalActiveStudents = studentStats.Total,
            MonthlyNewStudents = studentStats.Monthly,
            WeeklyNewStudents = studentStats.Weekly,
            TodayNewStudents = studentStats.Today,
            
            PendingPayments = manualPaymentStats.Pending,
            ApprovedPayments = manualPaymentStats.Approved,
            RejectedPayments = manualPaymentStats.Rejected,
            
            AverageOrderValue = generalStats.AverageOrderValue,
            SuccessRate = generalStats.SuccessRate,
            
            RevenueChart = revenueChart,
            PaymentMethodStats = paymentMethodStats
        };
    }

    private async Task<(long Total, long Monthly, long Weekly, long Today)> CalculateRevenueStats(
        DateTime today, DateTime weekStart, DateTime monthStart, DateTime yearStart, CancellationToken cancellationToken)
    {
        var completedTransactions = _transactionRepository.Table
            .Where(t => t.Status == TransactionStatus.Completed);

        var total = await completedTransactions.SumAsync(t => t.Amount, cancellationToken);
        var monthly = await completedTransactions.Where(t => t.CreatedAt >= monthStart).SumAsync(t => t.Amount, cancellationToken);
        var weekly = await completedTransactions.Where(t => t.CreatedAt >= weekStart).SumAsync(t => t.Amount, cancellationToken);
        var todayRevenue = await completedTransactions.Where(t => t.CreatedAt >= today).SumAsync(t => t.Amount, cancellationToken);

        return (total, monthly, weekly, todayRevenue);
    }

    private async Task<(int Total, int Monthly, int Weekly, int Today)> CalculateTransactionStats(
        DateTime today, DateTime weekStart, DateTime monthStart, DateTime yearStart, CancellationToken cancellationToken)
    {
        var completedTransactions = _transactionRepository.Table
            .Where(t => t.Status == TransactionStatus.Completed);

        var total = await completedTransactions.CountAsync(cancellationToken);
        var monthly = await completedTransactions.Where(t => t.CreatedAt >= monthStart).CountAsync(cancellationToken);
        var weekly = await completedTransactions.Where(t => t.CreatedAt >= weekStart).CountAsync(cancellationToken);
        var todayCount = await completedTransactions.Where(t => t.CreatedAt >= today).CountAsync(cancellationToken);

        return (total, monthly, weekly, todayCount);
    }

    private async Task<(int Total, int Monthly, int Weekly, int Today)> CalculateStudentStats(
        DateTime today, DateTime weekStart, DateTime monthStart, CancellationToken cancellationToken)
    {
        var activeEnrollments = _enrollmentRepository.Table
            .Where(e => e.EnrollmentStatus == EnrollmentStatus.Active);

        var total = await activeEnrollments.Select(e => e.StudentId).Distinct().CountAsync(cancellationToken);
        var monthly = await activeEnrollments.Where(e => e.CreatedAt >= monthStart).Select(e => e.StudentId).Distinct().CountAsync(cancellationToken);
        var weekly = await activeEnrollments.Where(e => e.CreatedAt >= weekStart).Select(e => e.StudentId).Distinct().CountAsync(cancellationToken);
        var todayCount = await activeEnrollments.Where(e => e.CreatedAt >= today).Select(e => e.StudentId).Distinct().CountAsync(cancellationToken);

        return (total, monthly, weekly, todayCount);
    }

    private async Task<(int Pending, int Approved, int Rejected)> CalculateManualPaymentStats(CancellationToken cancellationToken)
    {
        var pending = await _paymentAttemptRepository.Table
            .CountAsync(m => m.Status == PaymentAttemptStatus.PendingPayment || m.Status == PaymentAttemptStatus.AwaitingAdminApproval, cancellationToken);
        
        var approved = await _paymentAttemptRepository.Table
            .CountAsync(m => m.Status == PaymentAttemptStatus.Paid, cancellationToken);
        
        var rejected = await _paymentAttemptRepository.Table
            .CountAsync(m => m.Status == PaymentAttemptStatus.Failed, cancellationToken);

        return (pending, approved, rejected);
    }

    private async Task<(long AverageOrderValue, decimal SuccessRate)> CalculateGeneralStats(CancellationToken cancellationToken)
    {
        var completedOrders = await _orderRepository.Table
            .Where(o => o.Status == OrderStatus.Completed)
            .ToListAsync(cancellationToken);

        var totalOrders = await _orderRepository.Table.CountAsync(cancellationToken);
        var completedCount = completedOrders.Count;

        var averageOrderValue = completedOrders.Any() ? (long)completedOrders.Average(o => o.TotalAmount) : 0;
        var successRate = totalOrders > 0 ? (decimal)completedCount / totalOrders * 100 : 0;

        return (averageOrderValue, successRate);
    }

    private async Task<List<RevenueByDayDto>> GetRevenueChart(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var dailyRevenue = await _transactionRepository.Table
            .Where(t => t.Status == TransactionStatus.Completed && 
                       t.CreatedAt >= fromDate && 
                       t.CreatedAt <= toDate)
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new RevenueByDayDto
            {
                Date = g.Key,
                Revenue = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync(cancellationToken);

        return dailyRevenue;
    }

    private async Task<List<PaymentMethodStatsDto>> GetPaymentMethodStats(CancellationToken cancellationToken)
    {
        var methodStats = await _transactionRepository.Table
            .Where(t => t.Status == TransactionStatus.Completed)
            .GroupBy(t => t.Method)
            .Select(g => new
            {
                Method = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(t => t.Amount)
            })
            .ToListAsync(cancellationToken);

        var totalAmount = methodStats.Sum(s => s.TotalAmount);

        return methodStats.Select(s => new PaymentMethodStatsDto
        {
            Method = s.Method.ToString(),
            MethodDisplay = GetMethodDisplayText(s.Method),
            Count = s.Count,
            TotalAmount = s.TotalAmount,
            Percentage = totalAmount > 0 ? (decimal)s.TotalAmount / totalAmount * 100 : 0
        }).ToList();
    }

    private static string GetMethodDisplayText(Pardis.Domain.Accounting.PaymentMethod method)
    {
        return method switch
        {
            Pardis.Domain.Accounting.PaymentMethod.Online => "آنلاین",
            Pardis.Domain.Accounting.PaymentMethod.Wallet => "کیف پول",
            Pardis.Domain.Accounting.PaymentMethod.Cash => "نقدی",
            Pardis.Domain.Accounting.PaymentMethod.Transfer => "انتقال بانکی",
            _ => "نامشخص"
        };
    }
}