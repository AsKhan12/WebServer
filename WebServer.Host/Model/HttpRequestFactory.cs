namespace WebServer.Host.Model;

public static class HttpRequestFactory
{
    public static async Task<HttpRequest> CreateAsync(TcpStreamReader reader)
    {
        // --- Parse Request Line ---
        string requestLine = await reader.GetRequestLine();
        string[] requestParts = requestLine.Split(' ', 3);

        if (requestParts.Length != 3)
            throw new HttpRequestException("Invalid HTTP request line");

        string method = requestParts[0];
        string fullPath = requestParts[1];
        string version = requestParts[2];

        // Split path and query string
        string path = fullPath;
        string queryString = string.Empty;
        int queryIndex = fullPath.IndexOf('?');
        if (queryIndex >= 0)
        {
            path = fullPath.Substring(0, queryIndex);
            queryString = fullPath.Substring(queryIndex + 1);
        }

        // --- Parse Headers ---
        List<string> rawHeaders = await reader.GetHeaders();
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        int contentLength = 0;

        foreach (var header in rawHeaders)
        {
            int colonIndex = header.IndexOf(':');
            if (colonIndex <= 0)
                continue;

            string key = header.Substring(0, colonIndex).Trim();
            string value = header.Substring(colonIndex + 1).Trim();

            headers[key] = value;

            if (key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(value, out contentLength))
                    throw new InvalidDataException("Invalid Content-Length value.");
            }
        }

        // --- Parse Body ---
        string body = string.Empty;
        if (contentLength > 0)
        {
            body = await reader.GetBody(contentLength);
        }

        // --- Return Populated Request Object ---
        return new HttpRequest
        {
            Method = method,
            Path = path,
            QueryString = queryString,
            Version = version,
            Headers = headers,
            Body = body
        };
    }
}

