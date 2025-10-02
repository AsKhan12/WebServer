namespace WebServer.Host.Model;

public sealed class PipelineBuilder
{
    // Each middleware transforms the next delegate into a new delegate
    private readonly List<Func<RequestDelegate, RequestDelegate>> _components = new();

    // Add middleware
    public PipelineBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        _components.Add(middleware);
        return this;
    }

    // Build the final delegate: (((terminal ∘ mwN) ∘ mwN-1) ... ∘ mw1)
    public RequestDelegate Build(RequestDelegate terminal)
    {
        RequestDelegate app = terminal;
        for (int i = _components.Count - 1; i >= 0; i--)
        {
            app = _components[i](app);
        }
        return app;
    }
}
