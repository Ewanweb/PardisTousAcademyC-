using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ApplicationSeo = Pardis.Application.Seo;
using DomainSeo = Pardis.Domain.Seo;
using SeoDto = Pardis.Domain.Dto.Seo.SeoDto;
using System.Text.Json;

namespace Pardis.Infrastructure.Caching
{
    public interface ISeoCache
    {
        Task<SeoDto?> GetAsync(string key);
        Task SetAsync(string key, SeoDto seoDto, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        string BuildCacheKey(DomainSeo.SeoEntityType entityType, string slug, string language, int? page = null);
    }

    public class SeoCache : ISeoCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<SeoCache> _logger;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromHours(1);
        private readonly TimeSpan _memoryExpiry = TimeSpan.FromMinutes(15);

        public SeoCache(
            IDistributedCache distributedCache,
            IMemoryCache memoryCache,
            ILogger<SeoCache> logger)
        {
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<SeoDto?> GetAsync(string key)
        {
            try
            {
                // Try memory cache first (L1)
                if (_memoryCache.TryGetValue(key, out SeoDto? memoryCachedSeo))
                {
                    _logger.LogDebug("SEO cache hit (memory): {Key}", key);
                    return memoryCachedSeo;
                }

                // Try distributed cache (L2)
                var cachedJson = await _distributedCache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var seoDto = JsonSerializer.Deserialize<SeoDto>(cachedJson);
                    if (seoDto != null)
                    {
                        // Store in memory cache for faster access
                        _memoryCache.Set(key, seoDto, _memoryExpiry);
                        _logger.LogDebug("SEO cache hit (distributed): {Key}", key);
                        return seoDto;
                    }
                }

                _logger.LogDebug("SEO cache miss: {Key}", key);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SEO cache: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync(string key, SeoDto seoDto, TimeSpan? expiry = null)
        {
            try
            {
                var cacheExpiry = expiry ?? _defaultExpiry;
                var json = JsonSerializer.Serialize(seoDto);

                // Set in both caches
                _memoryCache.Set(key, seoDto, _memoryExpiry);
                await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheExpiry
                });

                _logger.LogDebug("SEO cached: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting SEO cache: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                await _distributedCache.RemoveAsync(key);
                _logger.LogDebug("SEO cache removed: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing SEO cache: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                // Note: This is a simplified implementation
                // In production with Redis, you would use SCAN with pattern matching
                _logger.LogWarning("Pattern-based cache invalidation not fully implemented for pattern: {Pattern}", pattern);
                
                // For now, we'll log the pattern for manual cleanup
                if (pattern.Contains("*"))
                {
                    _logger.LogWarning("Cache pattern invalidation requested: {Pattern}", pattern);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing SEO cache by pattern: {Pattern}", pattern);
            }
        }

        public string BuildCacheKey(DomainSeo.SeoEntityType entityType, string slug, string language, int? page = null)
        {
            var key = $"seo:{entityType.ToString().ToLower()}:{slug}:{language}";
            if (page.HasValue && page > 1)
            {
                key += $":page:{page}";
            }
            return key;
        }

        public async Task InvalidateAllAsync()
        {
            try
            {
                _logger.LogInformation("Invalidating all SEO cache");
                // Note: This would need to be implemented properly with Redis FLUSHDB or pattern matching
                // For now, we just log the action
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating all SEO cache");
            }
        }
    }

    // Background service for cache warming
    public class SeoCacheWarmupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeoCacheWarmupService> _logger;
        private readonly TimeSpan _warmupInterval = TimeSpan.FromHours(6);

        public SeoCacheWarmupService(IServiceProvider serviceProvider, ILogger<SeoCacheWarmupService> logger)
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
                    await WarmupCacheAsync();
                    await Task.Delay(_warmupInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SEO cache warmup service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task WarmupCacheAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var seoService = scope.ServiceProvider.GetService<ApplicationSeo.ISeoService>();
            
            if (seoService == null)
            {
                _logger.LogWarning("ISeoService not available for cache warmup");
                return;
            }

            _logger.LogInformation("Starting SEO cache warmup");

            try
            {
                // Warmup popular pages
                var popularSlugs = new[] { "home", "courses", "about", "contact" };
                
                foreach (var slug in popularSlugs)
                {
                    try
                    {
                        var context = new ApplicationSeo.SeoContext
                        {
                            BaseUrl = "https://localhost",
                            Language = "fa",
                            CurrentPath = "/",
                            QueryParams = new Dictionary<string, string>(),
                            Page = null,
                            IsPreview = false
                        };

                        await seoService.ResolveSeoBySlugAsync(DomainSeo.SeoEntityType.Page, slug, context);
                        _logger.LogDebug("Warmed up SEO cache for: {Slug}", slug);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to warm up SEO cache for: {Slug}", slug);
                    }
                }

                _logger.LogInformation("SEO cache warmup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during SEO cache warmup");
            }
        }
    }
}
