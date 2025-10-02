using System.Collections.Concurrent;

namespace WebServer.Host.Model;

public static class CachingMiddleware
{
    private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    public static Func<RequestDelegate, RequestDelegate> Use(TimeSpan? ttl = null)
    {
        var cacheTtl = ttl ?? TimeSpan.FromSeconds(30);

        return next => async (ctx) =>
        {
            string key = $"{ctx.Request.Method}:{ctx.Request.Path}";

            // Check cache hit
            if (_cache.TryGetValue(key, out var entry))
            {
                if (DateTime.UtcNow - entry.Timestamp < entry.Ttl)
                {
                    Console.WriteLine($"[CACHE HIT] {key}");
                    ctx.Response.StatusCode = entry.Response.StatusCode;
                    ctx.Response.ReasonPhrase = entry.Response.ReasonPhrase;
                    ctx.Response.Headers = new Dictionary<string, string>(entry.Response.Headers);
                    ctx.Response.Body = entry.Response.Body;
                    return;
                }
                else
                {
                    // Remove expired entry
                    _cache.TryRemove(key, out _);
                }
            }

            // Run pipeline and cache response
            await next(ctx);

            _cache[key] = new CacheEntry
            {
                Response = ctx.Response.Clone(), // Make sure to deep copy
                Timestamp = DateTime.UtcNow,
                Ttl = cacheTtl
            };
        };
    }

    private class CacheEntry
    {
        public HttpResponse Response { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public TimeSpan Ttl { get; set; }
    }
}

