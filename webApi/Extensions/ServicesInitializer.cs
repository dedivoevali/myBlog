using Common.Options;
using LanguageDetection;
using Microsoft.Extensions.Options;
using Service;
using Service.Abstract;
using Service.Abstract.Auth;
using Service.Abstract.Auth.Passkeys;
using Service.Abstract.Messaging;
using Service.Auth;
using Service.Auth.Passkeys;
using Service.Messaging;
using ProfanityFilter.Interfaces;
using Service.Abstract.Statistics;
using Service.Statistics;

namespace API.Extensions
{
    public static class ServicesInitializer
    {
        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageBus, MassTransitMessageBus>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFilePerUserHandlingService, AvatarService>();
            services.AddScoped<IPostReactionService, PostReactionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserPunishmentsService, UserPunishmentsService>();
            services.AddScoped<IContentStatisticsService, ContentStatisticsService>();
            
            // Passkey
            services.AddScoped<IPasskeyAuthService, PasskeyAuthService>();
            services.AddScoped<IPasskeyCryptographyService, PasskeyCryptographyService>();
            services.AddScoped<IPasskeySessionsService, PasskeySessionsService>();

            // Semantic analysis
            services.AddScoped<IProfanityFilter, ProfanityFilter.ProfanityFilter>();
            services.AddScoped<ISemanticAnalysisService, SemanticAnalysisService>(serviceProvider =>
            {
                var langDetector = new LanguageDetector();
                langDetector.AddAllLanguages();
                var profanityFilter = serviceProvider.GetRequiredService<IProfanityFilter>();
                var options = serviceProvider.GetRequiredService<IOptions<SemanticAnalysisOptions>>();
                return new SemanticAnalysisService(langDetector, profanityFilter, options);
            });
        }

        public static void InitializePasskeyFido2CryptoLibrary(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<PasskeyOptions>>().Value.RelyingParty;

            services.AddFido2(config =>
            {
                config.ServerDomain = options.DomainName;
                config.ServerName = options.DisplayName;
                config.ServerIcon = options.Icon;
                config.Origins = options.Origins.ToHashSet();
            });
        }
    }
}