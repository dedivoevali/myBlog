using System.Text.Json;
using Common.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Service.Abstract;

namespace Service;

public class RedisDistributedCacheService(IDistributedCache distributedCache, IOptions<CacheOptions> options) : ICacheService
{
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.DefaultExpirationInMinutes)
    };

    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken ct)
    {
        var stringValue = await distributedCache.GetStringAsync(cacheKey, ct);
        return string.IsNullOrWhiteSpace(stringValue) ? default! : JsonSerializer.Deserialize<T>(stringValue);
    }

    public async Task<string?> GetStringAsync(string cacheKey, CancellationToken ct)
    {
        var stringValue = await distributedCache.GetStringAsync(cacheKey, ct);

        return string.IsNullOrWhiteSpace(stringValue) ? default! : stringValue.Trim('"');
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken ct)
    {
        await distributedCache.RemoveAsync(cacheKey, ct);
    }

    public async Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        if (value is null)
            return;

        var serializedValue = JsonSerializer.Serialize(value);

        if (expiration is not null)
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
            await distributedCache.SetStringAsync(cacheKey, serializedValue, options , ct);
            return;
        }

        await distributedCache.SetStringAsync(cacheKey, serializedValue, _cacheOptions, ct);
    }
}