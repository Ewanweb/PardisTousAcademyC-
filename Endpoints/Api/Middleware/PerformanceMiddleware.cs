using System.Diagnostics;

namespace Api.Middleware
{
    /// <summary>
    /// Middleware برای مانیتورینگ عملکرد
    /// </summary>
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private readonly long _slowRequestThreshold;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _slowRequestThreshold = configuration.GetValue<long>("Performance:SlowRequestThresholdMs", 1000);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = context.Response.Headers["X-Request-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString("N")[..8];

            // Add performance headers
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
                context.Response.Headers["X-Server-Name"] = Environment.MachineName;
                return Task.CompletedTask;
            });

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var request = context.Request;
                var response = context.Response;

                // Log performance metrics
                var performanceData = new
                {
                    RequestId = requestId,
                    Method = request.Method,
                    Path = request.Path.Value,
                    StatusCode = response.StatusCode,
                    ElapsedMs = elapsedMs,
                    Memory = new
                    {
                        WorkingSet = GC.GetTotalMemory(false),
                        Gen0Collections = GC.CollectionCount(0),
                        Gen1Collections = GC.CollectionCount(1),
                        Gen2Collections = GC.CollectionCount(2)
                    },
                    Timestamp = DateTime.UtcNow
                };

                // Log slow requests
                if (elapsedMs > _slowRequestThreshold)
                {
                    _logger.LogWarning("Slow request detected: {PerformanceData}", 
                        System.Text.Json.JsonSerializer.Serialize(performanceData));
                }
                else
                {
                    _logger.LogDebug("Request performance: {PerformanceData}", 
                        System.Text.Json.JsonSerializer.Serialize(performanceData));
                }

                // Add custom metrics (you can integrate with monitoring tools like Prometheus, Application Insights, etc.)
                RecordMetrics(request.Method, request.Path.Value ?? "/", response.StatusCode, elapsedMs);
            }
        }

        private void RecordMetrics(string method, string path, int statusCode, long elapsedMs)
        {
            // Here you can integrate with your monitoring solution
            // For example: Prometheus, Application Insights, DataDog, etc.
            
            // Example: Custom metrics collection
            var metricsData = new
            {
                Method = method,
                Path = path,
                StatusCode = statusCode,
                ResponseTime = elapsedMs,
                Timestamp = DateTime.UtcNow
            };

            // In a real application, you might want to:
            // 1. Send to Prometheus
            // 2. Send to Application Insights
            // 3. Store in a time-series database
            // 4. Send to a monitoring service

            _logger.LogTrace("Metrics recorded: {MetricsData}", 
                System.Text.Json.JsonSerializer.Serialize(metricsData));
        }
    }

    /// <summary>
    /// Extension method برای اضافه کردن middleware
    /// </summary>
    public static class PerformanceMiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceMiddleware>();
        }
    }
}