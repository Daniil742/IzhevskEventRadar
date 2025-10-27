using IzhevskEventRadar.WebBot.Abstractions;

namespace IzhevskEventRadar.WebBot.Services;

internal class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
