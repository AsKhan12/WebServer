using System.Text;

namespace WebServer.Host.Model;

using System.Text.Json;

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string ReasonPhrase { get; set; } = "OK";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = "";

    public void SetText(string text)
    {
        Headers["Content-Type"] = "text/plain; charset=utf-8";
        Body = text;
        Headers["Content-Length"] = Encoding.UTF8.GetByteCount(text).ToString();
    }

    public void SetJson(object obj)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        Headers["Content-Type"] = "application/json; charset=utf-8";
        Body = json;
        Headers["Content-Length"] = Encoding.UTF8.GetByteCount(json).ToString();
    }

    // Deep copy to store in cache safely
    public HttpResponse Clone()
    {
        return new HttpResponse
        {
            StatusCode = this.StatusCode,
            ReasonPhrase = this.ReasonPhrase,
            Headers = new Dictionary<string, string>(this.Headers),
            Body = this.Body
        };
    }
}