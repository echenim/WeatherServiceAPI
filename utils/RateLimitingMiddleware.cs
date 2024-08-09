using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

/// <summary>
/// Middleware to limit the rate of requests from each IP address.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _requestLimit;
    private readonly TimeSpan _timeSpan;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the application's request pipeline.</param>
    /// <param name="requestLimit">The maximum number of allowed requests per time span.</param>
    /// <param name="timeSpan">The time span for which the request limit applies.</param>
    /// <param name="cache">The cache to store request counts.</param>
    /// <param name="logger">The logger for logging information about request limits.</param>
    public RateLimitingMiddleware(RequestDelegate next, int requestLimit, TimeSpan timeSpan, IMemoryCache cache, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _requestLimit = requestLimit;
        _timeSpan = timeSpan;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Invoked by the ASP.NET Core pipeline to handle the incoming HTTP request.
    /// </summary>
    /// <param name="context">The context for the current HTTP request.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress.ToString();
        var cacheKey = $"RateLimit_{ipAddress}";

        // Attempt to retrieve the current request count for the IP from the cache.
        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            // No entry in the cache indicates this is the first request from this IP.
            _cache.Set(cacheKey, 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _timeSpan // Start the expiration timer
            });
            _logger.LogInformation("Request count started for IP {IP}.", ipAddress);
        }
        else
        {
            if (requestCount >= _requestLimit)
            {
                // The IP has exceeded the allowed request limit within the specified time span.
                context.Response.StatusCode = 429; // HTTP status code for Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                _logger.LogWarning("Rate limit exceeded for IP {IP}.", ipAddress);
                return; // Stop further processing and return early
            }

            // Increment the request count and reset the cache expiration timer.
            _cache.Set(cacheKey, requestCount + 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _timeSpan
            });
            _logger.LogInformation("Request {RequestCount} for IP {IP}.", requestCount + 1, ipAddress);
        }

        // If the limit has not been reached, call the next middleware in the pipeline.
        await _next(context);
    }
}
