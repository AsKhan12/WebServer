using System.Net;
using WebServer.Host;
using WebServer.Host.Model;

var builder = new PipelineBuilder();

builder.Use(LoggingMiddleware.Use());
builder.Use(RateLimitingMiddleware.Use(100, TimeSpan.FromMinutes(5)));
builder.Use(CachingMiddleware.Use(TimeSpan.FromSeconds(10)));

var app = builder.Build(async ctx =>
    {
        ctx.Response.SetJson(new { message = "Hello, pooled world!" });
        await Task.CompletedTask;
    });

var server = new SimpleHttpServer(IPAddress.Loopback, 8080, app);
await server.StartAsync();
