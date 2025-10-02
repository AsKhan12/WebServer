using System.Text;

namespace WebServer.Host;

public class TcpStreamReader : IDisposable
{
    private readonly StreamReader _reader;
    private bool _disposed;

    public TcpStreamReader(Stream stream)
    {
        // Leave stream open control handled externally (pass false if you want this to close the stream too)
        _reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
    }

    public async Task<string> GetRequestLine()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(TcpStreamReader));
        return await _reader.ReadLineAsync() ?? throw new HttpRequestException("Invalid or empty request line");
    }

    public async Task<List<string>> GetHeaders()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(TcpStreamReader));

        string? line;
        List<string> headers = new();
        while (!string.IsNullOrEmpty(line = await _reader.ReadLineAsync()))
        {
            headers.Add(line);
            Console.WriteLine($"Header: {line}");
        }

        return headers;
    }

    public async Task<string> GetBody(int contentLength)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(TcpStreamReader));

        if (contentLength <= 0) return string.Empty;

        char[] buffer = new char[contentLength];
        int read = await _reader.ReadBlockAsync(buffer, 0, contentLength);
        if (read != contentLength)
            throw new IOException("Body length mismatch — incomplete read.");

        return new string(buffer);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _reader.Dispose();
        _disposed = true;
    }
}