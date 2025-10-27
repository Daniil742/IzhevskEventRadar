using IzhevskEventRadar.Domain.Configurations;
using IzhevskEventRadar.Infrastructure;

namespace IzhevskEventRadar.Api.BuilderExtensions;

public static class ServiceBuilder
{
    public static IServiceCollection AddServiceRegister(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AddOptionsRegister(serviceCollection, configuration);

        DependencyInjection.ConfigureServices(serviceCollection, configuration);

        return serviceCollection;
    }

    private static void AddOptionsRegister(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<VkApiOptions>()
            .Bind(configuration.GetSection(VkApiOptions.ConfigurationSectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddOptions<GeminiApiOptions>()
            .Bind(configuration.GetSection(GeminiApiOptions.ConfigurationSectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddOptions<RabbitMQOptions>()
            .Bind(configuration.GetSection(RabbitMQOptions.ConfigurationSectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
