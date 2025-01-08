using Common.Options;

namespace API.Extensions
{
    public static class CorsPolicyConfiguration
    {
        public static void AddCorsWithPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var policyConfiguration = configuration.GetSection(CorsPolicyOptions.Config).Get<CorsPolicyOptions>()
                ?? throw new ApplicationException("CORS Policy is not configured. Check appsettings.json");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .WithOrigins(policyConfiguration.AllowedOrigins)
                            .AllowAnyHeader()
                            .WithMethods(policyConfiguration.AllowedMethods);
                    });
            });
        }
    }
}