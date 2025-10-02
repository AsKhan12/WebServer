using System.Net.Sockets;
using System.Text;
using WebServer.Host.Model;

namespace WebServer.Host;

public static class TcpClientHandler
{
    public static async Task HandleClient(TcpClient client, RequestDelegate app)
    {
        using var stream = client.GetStream();
        using var reader = new TcpStreamReader(stream);
        var writer = new HttpResponseWriter(stream);

        try
        {
            var request = await HttpRequestFactory.CreateAsync(reader);
            var response = new HttpResponse();
            var context = new HttpContext(request, response);

            // Run through pipeline (no-op → terminal)
            await app(context);

            // Ensure a sane default
            context.Response.Headers["Connection"] = "close";

            await writer.WriteAsync(context.Response);
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex}");
            // Minimal error response
            var resp = new HttpResponse { StatusCode = 500, ReasonPhrase = "Internal Server Error" };
            resp.SetText("Server error");
            await new HttpResponseWriter(stream).WriteAsync(resp);
            client.Close();
        }
    }

}