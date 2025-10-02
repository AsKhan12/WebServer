namespace WebServer.Host.Model;

public static class LoggingMiddleware
{
    public static Func<RequestDelegate, RequestDelegate> Use()
    {
        return next => async ctx =>
        {
            var startTime = DateTime.UtcNow;

            // --- Log incoming request ---
            Console.WriteLine($"[REQUEST] {ctx.Request.Method} {ctx.Request.Path}");
            foreach (var header in ctx.Request.Headers)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }

            // --- Process pipeline ---
            await next(ctx);

            // --- Log response with elapsed time ---
            var elapsed = DateTime.UtcNow - startTime;
            Console.WriteLine($"[RESPONSE] {ctx.Response.StatusCode} {ctx.Response.ReasonPhrase} ({elapsed.TotalMilliseconds} ms)");
        };
    }
}

