using Azure.Storage.Blobs;
using Common.Options;
using Microsoft.Extensions.Options;

namespace API.Extensions;

public static class HealthChecksInitializer
{
    public static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var sql = configuration.GetConnectionString("Blog");
        var timeout = TimeSpan.FromSeconds(3);

        services.AddHealthChecks()
            .AddSqlServer(
                sql!,
                healthQuery: "SELECT 1 FROM dbo.[User]",
                timeout: timeout)
            .AddAzureBlobStorage(clientFactory: (provider) =>
            {
                var options = provider.GetService<IOptions<AzureStorageCredentialOptions>>().Value;
                return new BlobServiceClient(options.ConnectionString);
            }, timeout: timeout);


        return services;
    }
}
