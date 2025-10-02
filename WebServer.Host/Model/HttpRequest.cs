namespace WebServer.Host.Model;

public class HttpRequest
{
    public string Method { get; set; } = string.Empty;  // GET, POST, etc.
    public string Path { get; set; } = string.Empty;    // /hello
    public string QueryString { get; set; } = string.Empty; // ?name=asif
    public string Version { get; set; } = string.Empty; // HTTP/1.1
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = string.Empty;    // Optional for POST/PUT
}