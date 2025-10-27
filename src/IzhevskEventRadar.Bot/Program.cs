using IzhevskEventRadar.Domain.Configurations;
using IzhevskEventRadar.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<BotConfiguration>()
            .Bind(context.Configuration.GetSection(BotConfiguration.ConfigurationSectionName))
            //.ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient("telegram_bot_client")
            .RemoveAllLoggers()
            .AddTypedClient<ITelegramBotClient>((httpClient, provider) =>
            {
                var configuration = provider.GetService<IOptions<BotConfiguration>>()?.Value;

                ArgumentNullException.ThrowIfNull(configuration);

                var options = new TelegramBotClientOptions(configuration.BotToken);

                return new TelegramBotClient(options, httpClient);
            });

        //DependencyInjection.ConfigureServices(services);

    }).Build();

await host.RunAsync();