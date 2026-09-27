using StackExchange.Redis;

namespace Api.RateLimiting;

public class SlidingWindowRateLimiter(IConnectionMultiplexer _redis)
{
    public async Task<(bool allowed, int remaining)> CheckAsync(string identifier, int limit, int windowMs)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var key = $"ratelimit:{identifier}";
        var db = _redis.GetDatabase();

        var batch = db.CreateBatch();
        var addTask = batch.SortedSetAddAsync(key, $"{now}:{Random.Shared.NextDouble()}", now);
        var removeTask = batch.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, now - windowMs);
        var countTask = batch.SortedSetLengthAsync(key);
        var expireTask = batch.KeyExpireAsync(key, TimeSpan.FromMilliseconds(windowMs));
        batch.Execute();

        await Task.WhenAll(addTask, removeTask, expireTask);
        var count = (int)await countTask;

        return (count <= limit, Math.Max(0, limit - count));
    }
}