using IzhevskEventRadar.Contracts.Interfaces.Communicators;
using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.Domain.Configurations;
using IzhevskEventRadar.Infrastructure.Models.GeminiApiRequests;
using IzhevskEventRadar.Infrastructure.Models.GeminiApiResponses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace IzhevskEventRadar.Infrastructure.Services;

internal class GeminiApiCommunicator(
    IHttpClientFactory httpClientFactory,
    IOptions<GeminiApiOptions> options,
    ILogger<GeminiApiCommunicator> logger
    ) : IGeminiApiCommunicator
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly GeminiApiOptions _options = options.Value;
    private readonly ILogger<GeminiApiCommunicator> _logger = logger;

    public async Task<string> GenerateTextFromPromptAsync(string prompt, IReadOnlyCollection<string>? imageUrls, CancellationToken cancellationToken = default)
    {
        var parts = new List<Part> { new Part(Text: prompt) };

        using var client = _httpClientFactory.CreateClient();

        if (imageUrls is not null && imageUrls.Any())
        {
            foreach (var url in imageUrls)
            {
                try
                {
                    var imageBytes = await client.GetByteArrayAsync(url, cancellationToken);

                    var base64Image = Convert.ToBase64String(imageBytes);

                    var mimeType = url.EndsWith(".png")
                        ? "image/png"
                        : "image/jpeg";

                    parts.Add(new Part(InlineData: new InlineData(mimeType, base64Image)));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось скачать или обработать изображение по URL: {ImageUrl}", url);
                }
            }
        }

        var queryParams = new Dictionary<string, string>
        {
            ["key"] = _options.ApiKey
        };

        var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken);

        var geminiRequest = new GeminiApiRequest([new Content(parts)]);
        var requestUri = new Uri($"{_options.BaseUrl}/v1beta/models/{_options.Model}:generateContent?{queryString}");

        _logger.LogInformation("Отправка запроса к Gemini API. Prompt: {PromptText}, Images: {ImageCount}", prompt, imageUrls?.Count ?? 0);

        try
        {
            var response = await client.PostAsJsonAsync(requestUri, geminiRequest, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responsePayloadTest = await response.Content.ReadAsStringAsync();

            var responsePayload = await response.Content.ReadFromJsonAsync<GeminiApiResponse>(cancellationToken: cancellationToken);

            // Извлекаем текст из ответа. Он может быть вложенным.
            var responseText = responsePayload?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(responseText))
            {
                _logger.LogWarning("Gemini API вернул успешный ответ, но без текстового содержимого.");
                return string.Empty;
            }

            _logger.LogInformation("Успешный ответ от Gemini API.");
            return responseText;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении запроса к Gemini API. Статус-код: {StatusCode}", ex.StatusCode);

            throw;
        }
    }
}
