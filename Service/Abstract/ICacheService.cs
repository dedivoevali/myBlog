namespace Service.Abstract;

public interface ICacheService
{
    Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null, CancellationToken ct = default);
    Task<T?> GetAsync<T>(string cacheKey, CancellationToken ct = default);
    Task<string?> GetStringAsync(string cacheKey, CancellationToken ct = default);
    Task RemoveAsync(string cacheKey, CancellationToken ct = default);
}