using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _requestLimit;
    private readonly TimeSpan _timeSpan;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(RequestDelegate next, int requestLimit, TimeSpan timeSpan, IMemoryCache cache, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _requestLimit = requestLimit;
        _timeSpan = timeSpan;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress.ToString();
        var cacheKey = $"RateLimit_{ipAddress}";

        // Check if there's an entry in the cache for this IP
        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            // No entry in the cache means first request. Start counting.
            _cache.Set(cacheKey, 1, _timeSpan);
            _logger.LogInformation("Request count started for IP {IP}.", ipAddress);
        }
        else
        {
            if (requestCount >= _requestLimit)
            {
                // If the request count exceeds the limit, return 429 Too Many Requests
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                _logger.LogWarning("Rate limit exceeded for IP {IP}.", ipAddress);
                return;
            }

            // Increment the request count
            _cache.Set(cacheKey, requestCount + 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _timeSpan // Reset the time window
            });
            _logger.LogInformation("Request {RequestCount} for IP {IP}.", requestCount + 1, ipAddress);
        }

        // Proceed with the next middleware in the pipeline
        await _next(context);
    }
}
