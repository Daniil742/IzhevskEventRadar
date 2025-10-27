using IzhevskEventRadar.Contracts.Commands;
using IzhevskEventRadar.Contracts.Interfaces.Communicators;
using IzhevskEventRadar.Contracts.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace IzhevskEventRadar.Infrastructure.Consumers;

public class AiProcessingConsumer(
    IGeminiApiCommunicator geminiApiService,
    ILogger<AiProcessingConsumer> logger
    ) : IConsumer<PostFetchedEvent>
{
    private readonly IGeminiApiCommunicator _geminiApiService = geminiApiService;
    private readonly ILogger<AiProcessingConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<PostFetchedEvent> context)
    {
        var post = context.Message.Post;

        var promptTemplate = GetPromptTemplate();
        var fullPrompt = promptTemplate.Replace("{текст поста вконтакте}", post.Text);

        var imageUrls = post.Attachments?
            .Select(a => a.PhotoUrl)
            .Where(url => !string.IsNullOrEmpty(url))
            .ToArray();

        var response = await _geminiApiService.GenerateTextFromPromptAsync(fullPrompt, imageUrls, context.CancellationToken);

        if (string.IsNullOrEmpty(response) || response.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Gemini не нашел события в посте {PostId}.", post.Id);
            return;
        }

        var jsonResponse = Regex.Match(response, @"\{[\s\S]*\}");

        if (!jsonResponse.Success)
        {
            _logger.LogWarning("JSON object not found in input.");

            return;
        }

        EventDataDto? eventData;
        try
        {
            eventData = JsonConvert.DeserializeObject<EventDataDto>(jsonResponse.Value);

            if (eventData is null)
            {
                _logger.LogWarning("Не удалось десериализовать JSON от Gemini для поста {PostId}.", post.Id);
                return;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка парсинга JSON от Gemini для поста {PostId}.", post.Id);
            return;
        }

        var storeCommand = new StoreEventCommand(eventData, context.Message.InternalGroupId);
        var endpoint = await context.GetSendEndpoint(new Uri("queue:storage-consumer"));

        await endpoint.Send(storeCommand, context.CancellationToken);
    }

    private static string GetPromptTemplate()
    {
        return @"Ты — умный ассистент, который помогает находить события и структурировать информацию о них.

                Проанализируй текст и изображения ниже. Твоя задача — извлечь информацию о событии, сопоставить данные из текста и с афиши на картинках, и вернуть результат в формате JSON.
                
                Данные на картинках и в тексте могут не совпадать, то есть на картинке не обязательно будет присутствовать вся необходимая информация из текста и наоборот.
                Необходимо извлекать название мероприятия, дату проведения в любом формате, но в ответе приводить ее к шаблонной, если не указан год, значит год текущий, время начала, место проведения, организатора и краткое описание.
                
                По месту проведения нужно определить широту и долготу места проведения для указания на карте, эти параметры запиши в latitude и longitude результирующего JSON.

                [ШАБЛОН JSON]:
                {
                  ""title"": ""Название мероприятия"",
                  ""date"": ""Дата в формате ГГГГ-ММ-ДД"",
                  ""time"": ""Время начала в формате ЧЧ:ММ (если указано, иначе null)"",
                  ""location"": ""Место проведения (если указано, иначе null)"",
                  ""latitude"": ""Широта (если указано, иначе null)"",
                  ""longitude"": ""Долгота (если указано, иначе null)"",
                  ""organization"": ""Организатор (если указано, иначе null)"",
                  ""description"": ""Краткое описание события из текста""
                }

                Если в тексте и на изображениях нет информации о конкретном событии, верни null.

                Вот данные для анализа:
                [ТЕКСТ ПОСТА]:
                {текст поста вконтакте}";
    }
}
