using IzhevskEventRadar.WebBot.Abstractions;
using Telegram.Bot;

namespace IzhevskEventRadar.WebBot.Services;

internal class ReceiverService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<ReceiverServiceBase<UpdateHandler>> logger)
    : ReceiverServiceBase<UpdateHandler>(botClient, updateHandler, logger);
