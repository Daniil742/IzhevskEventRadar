using IzhevskEventRadar.Contracts.Interfaces.Communicators;
using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.Domain.Configurations;
using IzhevskEventRadar.Infrastructure.Models.VkApiResponses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IzhevskEventRadar.Infrastructure.Services;

internal class VkApiCommunicator(
    IHttpClientFactory httpClientFactory,
    IOptions<VkApiOptions> options,
    ILogger<VkApiCommunicator> logger
    ) : IVkApiCommunicator
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly VkApiOptions _options = options.Value;
    private readonly ILogger<VkApiCommunicator> _logger = logger;

    public async Task<IReadOnlyCollection<PostDto>> GetWallPostsAsync(string internalGroupId, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["domain"] = internalGroupId,
            ["count"] = "3",
            ["v"] = _options.ApiVersion
        };

        var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken);

        var requestUri = new Uri($"{_options.ApiUrl}/method/{VkApiMethods.WallGet.Value}?{queryString}");

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        _logger.LogInformation("Отправка запроса к VK API: {Method} {Uri}", "wall.get", internalGroupId);

        using var client = _httpClientFactory.CreateClient();

        try
        {
            var response = await client.SendAsync(httpRequestMessage, cancellationToken);

            _logger.LogInformation("Успешный ответ от VK API для {InternalGroupId}", internalGroupId);
            response.EnsureSuccessStatusCode();

            var responsePayloadTest = await response.Content.ReadAsStringAsync();

            var responsePayload = await response.Content.ReadFromJsonAsync<VkApiResponse>(cancellationToken);

            var result = responsePayload?.Response?.Items is null
                ? Array.Empty<PostDto>()
                : responsePayload.Response.Items
                    .Where(x => x.MarkedAsAds == 0)
                    .Select(x => new PostDto
                    {
                        Id = x.Id,
                        PublishDate = x.PublishDate,
                        Text = x.Text,
                        Attachments = (x.WallPostAttachments ?? Array.Empty<WallPostAttachment>())
                            .Where(a => string.Equals(a?.Type, "photo", StringComparison.OrdinalIgnoreCase) && a?.Photo != null)
                            .Select(a =>
                            {
                                var photo = a!.Photo!;
                                var photoUrl = photo.OriginalPhoto?.PhotoUrl;

                                return new AttachmentDto
                                {
                                    Type = a.Type,
                                    PhotoId = photo.Id,
                                    PhotoUrl = photoUrl
                                };
                            }).ToArray()
                    }).ToArray();

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении запроса к VK API для {InternalGroupId}. Статус-код: {StatusCode}", internalGroupId, ex.StatusCode);

            throw;
        }
    }
}
