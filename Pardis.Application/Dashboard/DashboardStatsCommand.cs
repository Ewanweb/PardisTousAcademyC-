using MediatR;
using Pardis.Domain;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Dashboard
{
    public record DashboardStatsCommand : IRequest<DashboardStatsDto>;

    // Pardis.Application/Dashboard/Queries/GetDashboardStatsHandler.cs

    public class DashboardStatsCommandHandler : IRequestHandler<DashboardStatsCommand, DashboardStatsDto>
    {
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Category> _catRepo;
        private readonly IRepository<User> _userRepo;

        public DashboardStatsCommandHandler(
            IRepository<Course> courseRepo,
            IRepository<Category> catRepo,
            IRepository<User> userRepo)
        {
            _courseRepo = courseRepo;
            _catRepo = catRepo;
            _userRepo = userRepo;
        }

        public async Task<DashboardStatsDto> Handle(DashboardStatsCommand request, CancellationToken cancellationToken)
        {
            // 1. آمار و درصد رشد
            var stats = new Dictionary<string, object>
            {
                { "students", await _userRepo.CountAsync() },
                { "students_trend", await CalculateGrowth(_userRepo) },
                { "courses", await _courseRepo.CountAsync() },
                { "courses_trend", await CalculateGrowth(_courseRepo) },
                { "categories", await _catRepo.CountAsync() },
                { "categories_trend", await CalculateGrowth(_catRepo) },
                // برای Revenue باید کوئری خاص زد، فعلا 0 میگذاریم
                { "revenue", 0 },
                { "revenue_trend", 0 }
            };

            // 2. فعالیت‌های اخیر

            // رفع خطا: تبدیل c به u در بخش کاربران
            var recentCourses = (await _courseRepo.GetLatestAsync(5)).Select(c => new RecentActivityDto
            {
                Id = "course_" + c.Id,
                Type = "course",
                Title = $"دوره جدید: {c.Title}",
                Subtitle = "...", // چون مدرس include نشده شاید نال باشد
                Time = c.CreatedAt
            });

            var recentUsers = (await _userRepo.GetLatestAsync(5)).Select(u => new RecentActivityDto
            {
                Id = "user_" + u.Id,
                Type = "user",
                Title = $"کاربر جدید: {u.FullName}",
                Subtitle = u.Email,
                Time = u.CreatedAt // رفع خطای The name 'c' does not exist
            });

            var recentCategories = (await _catRepo.GetLatestAsync(5)).Select(c => new RecentActivityDto
            {
                Id = "cat_" + c.Id,
                Type = "category",
                Title = $"دسته‌بندی: {c.Title}",
                Subtitle = "...",
                Time = c.CreatedAt
            });

            // ادغام و مرتب‌سازی
            var activities = recentCourses
                .Concat(recentUsers)
                .Concat(recentCategories)
                .OrderByDescending(x => x.Time)
                .Take(6)
                .Cast<object>() // تبدیل به object برای لیست جنریک
                .ToList();

            return new DashboardStatsDto { Stats = stats, RecentActivity = activities };
        }

        // تغییر: محدودیت T به IHasCreatedAt تغییر کرد (نه BaseEntity)
        private async Task<double> CalculateGrowth<T>(IRepository<T> repo) where T : class
        {
            var now = DateTime.UtcNow;
            var currentMonthCount = await repo.CountByDateAsync(now.Month, now.Year);

            var lastMonthDate = now.AddMonths(-1);
            var lastMonthCount = await repo.CountByDateAsync(lastMonthDate.Month, lastMonthDate.Year);

            if (lastMonthCount == 0) return currentMonthCount > 0 ? 100 : 0;
            return Math.Round(((double)(currentMonthCount - lastMonthCount) / lastMonthCount) * 100, 1);
        }
    }
}
