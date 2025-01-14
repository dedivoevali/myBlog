using Common.Options;
using Service;
using Service.Abstract;

namespace API.Extensions;

public static class CacheInitializer
{
    public static CacheOptions AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheOptions = GetOptions(services, configuration);
        services.AddOptions<CacheOptions>().Bind(configuration.GetSection(CacheOptions.Config));

        switch (cacheOptions.Provider)
        {
            case CacheProvider.Redis:
            {
                services.AddScoped<ICacheService, RedisDistributedCacheService>();
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                });
                break;
            }
            case CacheProvider.InMemory:
            default:
            {
                services.AddMemoryCache();
                services.AddScoped<ICacheService, InMemoryCacheService>();
                break;
            }
        }

        return cacheOptions;
    }

    public static CacheOptions GetOptions(IServiceCollection services, IConfiguration configuration)
    {
        var cacheOptions = new CacheOptions();
        configuration.GetSection(CacheOptions.Config).Bind(cacheOptions);
        return cacheOptions;
    }
}