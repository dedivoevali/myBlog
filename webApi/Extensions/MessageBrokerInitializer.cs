using Common.Options;
using MassTransit;

namespace API.Extensions;

public static class MessageBrokerInitializer
{
    public static MessageBrokerOptions AddOptions(IConfiguration configuration, IServiceCollection services)
    {
        var options = new MessageBrokerOptions();
        configuration.GetSection(MessageBrokerOptions.Config).Bind(options);
        services.AddOptions<MessageBrokerOptions>().Bind(
            configuration.GetSection(MessageBrokerOptions.Config)
        );
        return options;
    }

    public static void ConfigureInMemory(IBusRegistrationConfigurator busConfigurator)
    {
        busConfigurator.UsingInMemory((context, configurator) =>
        {
            configurator.UseDelayedMessageScheduler();
            configurator.MapProducers(context)
                .MapConsumers(context);
            configurator.ConfigureEndpoints(context);
        });
    }

    public static void ConfigureRabbitMq(IBusRegistrationConfigurator busConfigurator, IConfiguration configuration)
    {
        busConfigurator.UsingRabbitMq((context, configurator) =>
        {
            configurator.Host(configuration["MessageBus:Host"]!, h =>
            {
                h.Username(configuration["MessageBus:Username"]!);
                h.Password(configuration["MessageBus:Password"]!);
            });
            configurator.UseDelayedMessageScheduler();
            configurator.MapProducers(context)
                .MapConsumers(context);
            configurator.ConfigureEndpoints(context);
        });
    }
}