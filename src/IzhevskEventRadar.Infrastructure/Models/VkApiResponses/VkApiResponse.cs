using System.Text.Json.Serialization;

namespace IzhevskEventRadar.Infrastructure.Models.VkApiResponses;

internal class VkApiResponse
{
    [JsonPropertyName("response")]
    public WallGetResult? Response { get; set; }
}
