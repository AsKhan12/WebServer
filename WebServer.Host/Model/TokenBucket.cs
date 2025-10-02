namespace WebServer.Host.Model;

public class TokenBucket
{
    private readonly int _capacity;
    private int _tokens;
    private DateTime _lastRefill;
    private readonly TimeSpan _refillInterval;
    private readonly object _lock = new();

    public TokenBucket(int capacity, TimeSpan refillInterval)
    {
        _capacity = capacity;
        _tokens = capacity;
        _refillInterval = refillInterval;
        _lastRefill = DateTime.UtcNow;
    }

    public bool GrantToken()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            if (now - _lastRefill >= _refillInterval)
            {
                _tokens = _capacity;
                _lastRefill = now;
            }

            if (_tokens > 0)
            {
                _tokens--;
                return true;
            }
            return false;
        }
    }
}