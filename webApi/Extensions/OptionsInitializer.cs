using Common.Options;

namespace API.Extensions
{
    public static class OptionsInitializer
    {
        public static void InitializeOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JsonWebTokenOptions>(
                configuration.GetSection(JsonWebTokenOptions.Config));

            services.Configure<CorsPolicyOptions>(
                configuration.GetSection(CorsPolicyOptions.Config));

            services.Configure<PasskeyOptions>(
                configuration.GetSection(PasskeyOptions.Config));

            services.Configure<SemanticAnalysisOptions>(
                configuration.GetSection(SemanticAnalysisOptions.Config));

            services.Configure<PunishmentOptions>(
                configuration.GetSection(PunishmentOptions.Config));

            services.Configure<AzureStorageCredentialOptions>(
                configuration.GetSection(AzureStorageCredentialOptions.Config));

            services.Configure<AvatarSizingOptions>(
                configuration.GetSection(AvatarSizingOptions.Config));

            AddAzureContainerOptions(services, configuration);
        }

        private static void AddAzureContainerOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AvatarContainerOptions>(
                configuration.GetSection("BlobContainers:Avatar"));
        }
    }
}