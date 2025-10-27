using IzhevskEventRadar.WebBot.Configurations;
using IzhevskEventRadar.WebBot.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddOptions<BotConfiguration>()
    .Bind(builder.Configuration.GetSection(BotConfiguration.ConfigurationSectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

#region Bot Dependencies

builder.Services.AddHttpClient("telegram_bot_client")
    .RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, provider) =>
    {
        var configuration = provider.GetService<IOptions<BotConfiguration>>()?.Value;

        ArgumentNullException.ThrowIfNull(configuration);

        var options = new TelegramBotClientOptions(configuration.BotToken);

        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();

builder.Services.AddHostedService<PollingService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
