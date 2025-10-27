using IzhevskEventRadar.Contracts.Interfaces;
using IzhevskEventRadar.Contracts.Interfaces.Communicators;
using IzhevskEventRadar.Domain.Configurations;
using IzhevskEventRadar.Infrastructure.Consumers;
using IzhevskEventRadar.Infrastructure.Consumers.ConsumerDefinitions;
using IzhevskEventRadar.Infrastructure.Interfaces;
using IzhevskEventRadar.Infrastructure.Services;
using IzhevskEventRadar.Infrastructure.Services.Internal;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace IzhevskEventRadar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        #region VK API

        serviceCollection.AddHttpClient<IVkApiCommunicator, VkApiCommunicator>((provider, client) =>
        {
            var opts = provider
                .GetRequiredService<IOptions<VkApiOptions>>()
                .Value;

            client.BaseAddress = new Uri(opts.ApiUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", opts.AccessToken);
        });

        serviceCollection.AddScoped<IVkApiCommunicator, VkApiCommunicator>();

        #endregion

        #region Business Services Register

        serviceCollection.AddScoped<IGroupRepository, GroupRepository>();
        serviceCollection.AddScoped<IGroupService, GroupService>();

        serviceCollection.AddScoped<IPostRepository, PostRepository>();
        serviceCollection.AddScoped<IPostService, PostService>();

        serviceCollection.AddScoped<IEventRepository, EventRepository>();
        serviceCollection.AddScoped<IEventService, EventService>();

        #endregion

        #region MassTransit Register

        serviceCollection.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<ProcessAllGroupsConsumer>();//ProcessAllGroupsConsumerDefinition
            configurator.AddConsumer<FetchPostsConsumer>();//FetchPostsConsumerDefinition
            configurator.AddConsumer<AiProcessingConsumer>();//AiProcessingConsumerDefinition
            configurator.AddConsumer<StorageConsumer>();//StorageConsumerDefinition

            configurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetValue<string>("RabbitMQ:Uri");

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Строка подключения 'RabbitMQ' не найдена в конфигурации.");

                rabbitConfigurator.Host(connectionString);

                //var uri = new Uri(connectionString);

                //rabbitConfigurator.Host(uri.Host, 5671, uri.AbsolutePath.Trim('/'), h =>
                //{
                //    h.UseSsl();
                //    h.Username(uri.UserInfo.Split(':')[0]);
                //    h.Password(uri.UserInfo.Split(':')[1]);
                //});

                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });

        #endregion

        #region Ginimi API

        serviceCollection.AddHttpClient<IGeminiApiCommunicator, GeminiApiCommunicator>((provider, client) =>
        {
            var opts = provider
                .GetRequiredService<IOptions<GeminiApiOptions>>()
                .Value;

            client.BaseAddress = new Uri(opts.BaseUrl);
        });

        serviceCollection.AddScoped<IGeminiApiCommunicator, GeminiApiCommunicator>();

        #endregion

        return serviceCollection;
    }
}
