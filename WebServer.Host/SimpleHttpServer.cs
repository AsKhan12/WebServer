using System.Net;
using System.Net.Sockets;
using WebServer.Host.Model;

namespace WebServer.Host;

public class SimpleHttpServer
{
    private readonly TcpListener _listener;
    private readonly RequestDelegate _pipeline;

    public SimpleHttpServer(
        IPAddress ip,
        int port,
        RequestDelegate pipeline
        )
    {
        _listener = new TcpListener(ip, port);
        _pipeline = pipeline;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listener.Start();
        Console.WriteLine("Server started. Listening for connections...");

        while (!cancellationToken.IsCancellationRequested)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
            _ = ProcessConnectionAsync(tcpClient, cancellationToken);
        }
    }

    private async Task ProcessConnectionAsync(TcpClient client, CancellationToken cancellationToken)
    {
        try
        {
            using NetworkStream stream = client.GetStream();

            // --- Read HTTP request
            var reader = new TcpStreamReader(stream);
            HttpRequest request = await HttpRequestFactory.CreateAsync(reader);


            // --- Build HttpContext
            var context = new HttpContext(request, new HttpResponse());

            // --- Pass to pipeline
            await _pipeline(context);

            // --- Send response
            var writer = new HttpResponseWriter(stream);
            await writer.WriteAsync(context.Response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
    }
}