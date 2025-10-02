using System.Text;
using WebServer.Host.Model;

namespace WebServer.Host;

public class HttpResponseWriter
{
    private readonly Stream _stream;

    public HttpResponseWriter(Stream stream)
    {
        _stream = stream;
    }

    public async Task WriteAsync(HttpResponse response)
    {
        // --- Convert body to bytes first ---
        byte[] bodyBytes = Encoding.UTF8.GetBytes(response.Body);

        // --- Ensure content-length is set correctly ---
        response.Headers["Content-Length"] = bodyBytes.Length.ToString();

        // --- Build the header string ---
        var headerBuilder = new StringBuilder();
        headerBuilder.Append($"HTTP/1.1 {response.StatusCode} {response.ReasonPhrase}\r\n");

        foreach (var header in response.Headers)
        {
            headerBuilder.Append($"{header.Key}: {header.Value}\r\n");
        }

        // --- End headers with CRLF ---
        headerBuilder.Append("\r\n");

        // --- Write headers ---
        byte[] headerBytes = Encoding.UTF8.GetBytes(headerBuilder.ToString());
        await _stream.WriteAsync(headerBytes, 0, headerBytes.Length);

        // --- Write body ---
        if (bodyBytes.Length > 0)
        {
            await _stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
        }

        // --- Flush to ensure data is sent ---
        await _stream.FlushAsync();
    }
}
