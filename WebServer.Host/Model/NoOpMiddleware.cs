namespace WebServer.Host.Model;

// Does nothing except pass control to the next component
public static class NoOpMiddleware
{
    public static Func<RequestDelegate, RequestDelegate> Use() =>
        next => async ctx =>
        {
            // place to inspect/modify ctx.Request/ctx.Response
            await next(ctx);
        };
}
