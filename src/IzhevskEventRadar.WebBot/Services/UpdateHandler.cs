using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IzhevskEventRadar.WebBot.Services;

internal class UpdateHandler(
    ITelegramBotClient bot,
    ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private static Dictionary<string, DateOnly> _dateOptions = new();

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var handler = update switch
        {
            { Message: { } message } => OnMessageReceived(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => OnCallbackQueryReceived(callbackQuery, cancellationToken),
            _ => Task.CompletedTask
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, HandleErrorSource.HandleUpdateError, cancellationToken);
        }
    }

    #region Privates

    async Task OnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Receive message type: {MessageType}", message.Type);

        if (message.Text is not { } messageText)
            return;

        var dateKeyboard = GenerateDateKeyboard();

        var action = messageText switch
        {
            "/start" => SendWelcomeMessage(message, dateKeyboard, cancellationToken),
            _ when _dateOptions.ContainsKey(messageText) => ShowEventsForDate(message, messageText, cancellationToken),
            _ => SendDefaultResponse(message, dateKeyboard, cancellationToken)
        };

        Message sentMessage = await action;
        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> SendWelcomeMessage(Message message, ReplyKeyboardMarkup keyboard, CancellationToken cancellationToken)
    {
        return await bot.SendMessage(
            chatId: message.Chat.Id,
            text: "Привет! 👋 Выбери дату, чтобы посмотреть события:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }

    async Task<Message> ShowEventsForDate(Message message, string selectedDateText, CancellationToken cancellationToken)
    {
        // Получаем объект DateOnly из словаря
        if (!_dateOptions.TryGetValue(selectedDateText, out var selectedDate))
        {
            // На всякий случай, если что-то пошло не так
            return await SendDefaultResponse(message, GenerateDateKeyboard(), cancellationToken);
        }

        logger.LogInformation("User selected date: {SelectedDate}", selectedDate.ToString("yyyy-MM-dd"));

        // --- ЗАГЛУШКА ДАННЫХ ---
        // В будущем здесь будет вызов вашего IEventService
        var fakeEvents = GetFakeEvents(selectedDate);
        // --- КОНЕЦ ЗАГЛУШКИ ---

        // Формируем сообщение
        var messageBuilder = new StringBuilder();
        var inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();

        var dateText = Regex.Escape(selectedDateText.Split(',')[0]);
        messageBuilder.AppendLine($"📅 *События на {selectedDate:dd MMMM}* \\({dateText}\\):\n");

        if (!fakeEvents.Any())
        {
            messageBuilder.AppendLine("_На эту дату событий не найдено\\._");
        }
        else
        {
            int eventIndex = 1; // Номер для отображения
            foreach (var ev in fakeEvents)
            {
                var title = EscapeMarkdownV2(ev.Title);
                var location = EscapeMarkdownV2(ev.Location);

                // Добавляем номер к событию
                messageBuilder.AppendLine($"*{eventIndex}\\. {title}*");
                messageBuilder.AppendLine($"📍 _{location}_");
                messageBuilder.AppendLine($"⏰ {ev.Date:HH:mm}");
                messageBuilder.AppendLine();

                // Создаем кнопку для этого события
                // CallbackData будет содержать префикс "event_" и ID события
                inlineKeyboardButtons.Add(new List<InlineKeyboardButton> {
                    InlineKeyboardButton.WithCallbackData($"Подробнее о №{eventIndex}", $"event_{ev.Id}")
                });
                eventIndex++;
            }
        }

        // Создаем разметку клавиатуры
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

        // --- ЗАГЛУШКА КАРТЫ ---
        // Формируем URL для статической карты (например, Yandex Static Maps API - бесплатно)
        // https://yandex.ru/dev/staticmaps/doc/ru/concepts/markers
        string mapUrl = GenerateStaticMapUrl(fakeEvents);

        // Отправляем карту (если есть что показывать)
        if (fakeEvents.Any())
        {
            try
            {
                await bot.SendPhoto(
                    chatId: message.Chat.Id,
                    photo: InputFile.FromUri(mapUrl),
                    caption: "Места проведения на карте",
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // Если URL карты невалиден или недоступен, просто логируем
                logger.LogWarning(ex, "Could not send static map photo.");
            }
        }
        // --- КОНЕЦ ЗАГЛУШКИ КАРТЫ ---

        // Отправляем текстовое сообщение
        return await bot.SendMessage(
            chatId: message.Chat.Id,
            text: messageBuilder.ToString(),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: inlineKeyboardMarkup,
            cancellationToken: cancellationToken);
    }

    async Task OnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        // Убедимся, что данные пришли и есть сообщение, к которому они привязаны
        if (string.IsNullOrEmpty(callbackQuery.Data) || callbackQuery.Message is null)
            return;

        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}, data: {Data}", callbackQuery.Id, callbackQuery.Data);

        // Отвечаем серверу Telegram, что мы получили нажатие (чтобы убрать "часики" на кнопке)
        await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);

        // Парсим CallbackData
        if (callbackQuery.Data.StartsWith("event_"))
        {
            // Извлекаем ID события
            if (int.TryParse(callbackQuery.Data.Split('_')[1], out int eventId))
            {
                // --- ЗАГЛУШКА ДАННЫХ ---
                // В будущем здесь будет вызов IEventService.GetEventByIdAsync(eventId)
                var eventDetails = GetFakeEventDetails(eventId);
                // --- КОНЕЦ ЗАГЛУШКИ ---

                if (eventDetails != null)
                {
                    // Формируем сообщение с деталями
                    var detailsText = new StringBuilder();
                    detailsText.AppendLine($"*Детали события: {EscapeMarkdownV2(eventDetails.Title)}*\n");
                    // TODO: Добавить описание, если оно есть в DTO
                    // detailsText.AppendLine($"{EscapeMarkdownV2(eventDetails.Description)}\n"); 
                    detailsText.AppendLine($"*Дата:* {eventDetails.Date:dd MMMM yyyy, HH:mm}");
                    detailsText.AppendLine($"*Место:* {EscapeMarkdownV2(eventDetails.Location)}");

                    // Отправляем новое сообщение с деталями
                    await bot.SendMessage(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: detailsText.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        cancellationToken: cancellationToken);

                    // Альтернатива: можно отредактировать исходное сообщение, убрав кнопки:
                    // await botClient.EditMessageTextAsync(
                    //    callbackQuery.Message.Chat.Id, 
                    //    callbackQuery.Message.MessageId, 
                    //    detailsText.ToString(), 
                    //    parseMode: ParseMode.MarkdownV2, 
                    //    cancellationToken: cancellationToken);    
                }
                else
                {
                    await bot.SendMessage(callbackQuery.Message.Chat.Id, "Не удалось найти детали этого события.", cancellationToken: cancellationToken);
                }
            }
        }
        // Здесь можно добавить обработку других типов CallbackData, если они появятся
    }

    async Task<Message> SendDefaultResponse(Message message, ReplyKeyboardMarkup keyboard, CancellationToken cancellationToken)
    {
        return await bot.SendMessage(
            chatId: message.Chat.Id,
            text: "Используй кнопки ниже, чтобы выбрать дату.",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }

    ReplyKeyboardMarkup GenerateDateKeyboard()
    {
        _dateOptions.Clear(); // Очищаем старые опции
        var today = DateOnly.FromDateTime(DateTime.Now);
        var buttons = new List<List<KeyboardButton>>();
        var currentRow = new List<KeyboardButton>();

        for (int i = 0; i < 7; i++)
        {
            var date = today.AddDays(i);
            // Формат для отображения: "23.10 (чт)"
            string buttonText = date.ToString("dd.MM (ddd)", new CultureInfo("ru-RU")).ToLower();

            // Запоминаем соответствие текста и даты
            _dateOptions[buttonText] = date;

            currentRow.Add(new KeyboardButton(buttonText));

            // Переносим на новую строку каждые 3 кнопки
            if (currentRow.Count == 3 || i == 6)
            {
                buttons.Add(currentRow);
                currentRow = new List<KeyboardButton>();
            }
        }

        return new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true, // Делает клавиатуру компактной
            IsPersistent = true // Клавиатура остается видимой
            // OneTimeKeyboard = false // Тоже самое, что IsPersistent = true
        };
    }

    EventDto? GetFakeEventDetails(int eventId)
    {
        // Имитируем получение данных по ID. В реальности будет запрос к БД/сервису.
        // Используем DayNumber из ID, чтобы найти дату, к которой относится событие
        var date = DateOnly.FromDayNumber(eventId / 100);
        var fakeEventsOnDate = GetFakeEvents(date);
        return fakeEventsOnDate.FirstOrDefault(e => e.Id == eventId);
    }

    List<EventDto> GetFakeEvents(DateOnly date)
    {
        // Имитируем разные события для разных дней
        var random = new Random(date.DayNumber); // Используем DayNumber для стабильного "рандома" на одну дату
        int eventCount = random.Next(0, 4); // От 0 до 3 событий

        if (eventCount == 0) return new List<EventDto>();

        // Координаты Ижевска +-
        double baseLat = 56.85;
        double baseLng = 53.21;

        var events = new List<EventDto>();
        for (int i = 0; i < eventCount; i++)
        {
            var hour = random.Next(10, 22); // Время события
            var minute = random.Next(0, 2) * 30; // 00 или 30 минут
            events.Add(new EventDto(
                Id: date.DayNumber * 100 + i, // Уникальный ID
                Title: $"Тестовое Событие {i + 1} на {date:dd.MM}",
                Date: date.ToDateTime(new TimeOnly(hour, minute)),
                Location: $"Место {i + 1}, ул. Тестовая {random.Next(1, 100)}",
                Latitude: baseLat + random.NextDouble() * 0.05 - 0.025, // Небольшой разброс
                Longitude: baseLng + random.NextDouble() * 0.1 - 0.05
            ));
        }
        return events;
    }

    string GenerateStaticMapUrl(List<EventDto> events)
    {
        if (!events.Any()) return string.Empty;

        // Используем Yandex Static Maps API (бесплатно)
        // https://yandex.ru/dev/staticmaps/doc/ru/concepts/markers

        // Координаты центра и масштаб
        double avgLat = events.Average(e => e.Latitude);
        double avgLng = events.Average(e => e.Longitude);
        string centerLngLat = $"{avgLng.ToString(CultureInfo.InvariantCulture)},{avgLat.ToString(CultureInfo.InvariantCulture)}";
        string zoom = events.Count > 1 ? "12" : "14"; // Приближаем, если событие одно

        // Собираем метки: pt=lon1,lat1,pm2rdm~lon2,lat2,pm2blm~...
        // pm2rdm - красная метка, pm2blm - синяя и т.д.
        // Добавим номер в метку: pm2rdl1 (красная метка с цифрой 1)
        string markers = string.Join("~", events.Select((e, index) =>
            $"{e.Longitude.ToString(CultureInfo.InvariantCulture)},{e.Latitude.ToString(CultureInfo.InvariantCulture)},pm2rdl{index + 1}"
        ));

        // Формируем URL (размер карты 600x400)
        return $"https://static-maps.yandex.ru/1.x/?lang=ru_RU&ll={centerLngLat}&z={zoom}&l=map&size=600,400&pt={markers}";
    }

    // Вспомогательная функция для экранирования символов MarkdownV2
    string EscapeMarkdownV2(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        // Список символов для экранирования по документации Telegram
        var escapeChars = new[] { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };
        var result = new StringBuilder();
        foreach (char c in input)
        {
            if (escapeChars.Contains(c))
            {
                result.Append('\\');
            }
            result.Append(c);
        }
        return result.ToString();
    }

    Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    #endregion

    public record EventDto(
    int Id,
    string Title,
    DateTime Date,
    string Location,
    double Latitude,
    double Longitude
);
}
