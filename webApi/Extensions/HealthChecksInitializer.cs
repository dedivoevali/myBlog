using Azure.Storage.Blobs;
using Common.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace API.Extensions;

public static class HealthChecksInitializer
{
    public static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var sql = configuration.GetConnectionString("Blog");
        var timeout = TimeSpan.FromSeconds(3);
        var redis = CacheInitializer.GetOptions(services, configuration).Provider == CacheProvider.Redis ?
            configuration.GetConnectionString("Redis") :
            string.Empty;


        var builder = services.AddHealthChecks()
            .AddSqlServer(
                sql!,
                healthQuery: "SELECT 1 FROM dbo.[User]",
                timeout: timeout)
            .AddAzureBlobStorage(clientFactory: (provider) =>
            {
                var options = provider.GetService<IOptions<AzureStorageCredentialOptions>>()?.Value;
                ArgumentNullException.ThrowIfNull(options);
                return new BlobServiceClient(options.ConnectionString);
            }, timeout: timeout);

        if (!string.IsNullOrWhiteSpace(redis))
        {
            builder.AddRedis(redis, "redis", HealthStatus.Unhealthy, timeout: timeout, tags: [ "cache" ]);
        }

        return services;
    }
}
