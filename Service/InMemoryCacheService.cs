using Common.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Service.Abstract;

namespace Service;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public InMemoryCacheService(IMemoryCache memoryCache, IOptions<CacheOptions> options)
    {
        _memoryCache = memoryCache;
        _cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(options.Value.DefaultExpirationInMinutes)
        };
    }

    public Task<T?> GetAsync<T>(string cacheKey, CancellationToken ct = default)
    {
        return Task.FromResult(_memoryCache.Get<T?>(cacheKey));
    }

    public Task<string?> GetStringAsync(string cacheKey, CancellationToken ct = default)
    {
        var result = _memoryCache.Get(cacheKey);
        return Task.FromResult(result is not null ? result as string : default!);
    }

    public Task RemoveAsync(string cacheKey, CancellationToken ct = default)
    {
        _memoryCache.Remove(cacheKey);

        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        if (expiration is not null)
        {
            _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiration.Value
            });

            return Task.CompletedTask;
        }

        _memoryCache.Set(cacheKey, value, _cacheOptions);

        return Task.CompletedTask;
    }
}