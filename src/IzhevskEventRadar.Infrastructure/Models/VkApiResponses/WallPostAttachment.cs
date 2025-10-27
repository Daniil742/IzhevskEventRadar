using System.Text.Json.Serialization;

namespace IzhevskEventRadar.Infrastructure.Models.VkApiResponses;

internal class WallPostAttachment
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("photo")]
    public PhotoAttachment? Photo { get; set; }
}
