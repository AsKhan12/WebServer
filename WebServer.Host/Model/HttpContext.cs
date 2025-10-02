namespace WebServer.Host.Model;

public sealed class HttpContext
{
    public HttpRequest Request { get; }
    public HttpResponse Response { get; }

    // You can stash per-request state here later (e.g., user, items, features)
    public Dictionary<string, object> Items { get; } = new();

    public HttpContext(HttpRequest request, HttpResponse response)
    {
        Request = request;
        Response = response;
    }
}

public delegate Task RequestDelegate(HttpContext context);
