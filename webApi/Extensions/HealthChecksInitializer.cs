namespace API.Extensions;

public static class HealthChecksInitializer
{
    public static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var sql = configuration.GetConnectionString("Blog");

        services.AddHealthChecks()
            .AddSqlServer(
                sql!,
                healthQuery: "SELECT 1 FROM dbo.[User]",
                timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
