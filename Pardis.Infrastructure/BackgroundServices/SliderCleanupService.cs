using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pardis.Infrastructure.BackgroundServices
{
    public class SliderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SliderCleanupService> _logger;

        public SliderCleanupService(IServiceProvider serviceProvider, ILogger<SliderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredContent();
                    
                    // اجرا هر ساعت
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در پاک‌سازی محتوای منقضی شده");
                    
                    // در صورت خطا، 10 دقیقه صبر کن و دوباره تلاش کن
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }

        private async Task CleanupExpiredContent()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.UtcNow;

            // پاک‌سازی اسلایدهای منقضی شده
            var expiredHeroSlides = await context.HeroSlides
                .Where(x => !x.IsPermanent && x.ExpiresAt.HasValue && x.ExpiresAt.Value <= now)
                .ToListAsync();

            if (expiredHeroSlides.Count > 0)
            {
                context.HeroSlides.RemoveRange(expiredHeroSlides);
                _logger.LogInformation("پاک‌سازی {Count} اسلاید منقضی شده", expiredHeroSlides.Count);
            }

            // پاک‌سازی استوری‌های منقضی شده
            var expiredSuccessStories = await context.SuccessStories
                .Where(x => !x.IsPermanent && x.ExpiresAt.HasValue && x.ExpiresAt.Value <= now)
                .ToListAsync();

            if (expiredSuccessStories.Count > 0)
            {
                context.SuccessStories.RemoveRange(expiredSuccessStories);
                _logger.LogInformation("پاک‌سازی {Count} استوری منقضی شده", expiredSuccessStories.Count);
            }

            if (expiredHeroSlides.Count > 0 || expiredSuccessStories.Count > 0)
            {
                await context.SaveChangesAsync();
                _logger.LogInformation("پاک‌سازی محتوای منقضی شده با موفقیت انجام شد");
            }
        }
    }
}