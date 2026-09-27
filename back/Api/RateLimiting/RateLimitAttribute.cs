namespace Api.RateLimiting;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : Attribute
{
    public int Limit { get; }
    public int WindowMs { get; }

    public RateLimitAttribute(int limit, int windowMs)
    {
        Limit = limit;
        WindowMs = windowMs;
    }
}