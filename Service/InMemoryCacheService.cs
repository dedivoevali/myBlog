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
        _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
            TimeSpan.FromMinutes(options.Value.DefaultExpirationInMinutes)
        );
    }

    public Task<T?> GetAsync<T>(string cacheKey)
    {
        return Task.FromResult(_memoryCache.Get<T?>(cacheKey));
    }

    public Task<string?> GetStringAsync(string cacheKey)
    {
        var result = _memoryCache.Get(cacheKey);
        return Task.FromResult(result is not null ? result as string : default!);
    }

    public Task RemoveAsync(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);

        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null)
    {
        if (expiration is not null)
        {
            _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions().SetSlidingExpiration(expiration.Value));

            return Task.CompletedTask;
        }

        _memoryCache.Set(cacheKey, value, _cacheOptions);

        return Task.CompletedTask;
    }
}