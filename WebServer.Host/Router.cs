using WebServer.Host.Model;

namespace WebServer.Host;

public class Router
{
    private readonly Dictionary<(string Method, string Path), Func<HttpRequest, HttpResponse>> _routes
        = [];

    // Register a route
    public void Register(string method, string path, Func<HttpRequest, HttpResponse> handler)
    {
        _routes[(method, path)] = handler;
    }

    // Resolve a route
    public HttpResponse Route(HttpRequest request)
    {
        if (_routes.TryGetValue((request.Method, request.Path), out var handler))
        {
            return handler(request);
        }

        // Default 404 response if route not found
        var response = new HttpResponse();
        response.StatusCode = 404;
        response.ReasonPhrase = "Not Found";
        response.SetText($"Route {request.Method} {request.Path} not found.");
        return response;
    }
}