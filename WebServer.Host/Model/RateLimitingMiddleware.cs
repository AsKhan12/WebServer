using System.Collections.Concurrent;

namespace WebServer.Host.Model;

public static class RateLimitingMiddleware
{
    private static readonly ConcurrentDictionary<string, TokenBucket> _buckets = new();

    public static Func<RequestDelegate, RequestDelegate> Use(int capacity = 5, TimeSpan? refillInterval = null)
    {
        var interval = refillInterval ?? TimeSpan.FromSeconds(1);

        return next => async ctx =>
        {
            string key = ctx.Request.Headers.ContainsKey("X-Forwarded-For")
                ? ctx.Request.Headers["X-Forwarded-For"]
                : "local";

            var bucket = _buckets.GetOrAdd(key, _ => new TokenBucket(capacity, interval));

            if (!bucket.GrantToken())
            {
                ctx.Response.StatusCode = 429;
                ctx.Response.ReasonPhrase = "Too Many Requests";
                ctx.Response.SetText("Rate limit exceeded. Try again later.");
                return;
            }

            await next(ctx);
        };
    }
}