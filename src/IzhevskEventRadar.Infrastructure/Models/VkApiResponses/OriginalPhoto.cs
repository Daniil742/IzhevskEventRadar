using System.Text.Json.Serialization;

namespace IzhevskEventRadar.Infrastructure.Models.VkApiResponses;

internal class OriginalPhoto
{
    [JsonPropertyName("url")]
    public string? PhotoUrl { get; set; }
}
