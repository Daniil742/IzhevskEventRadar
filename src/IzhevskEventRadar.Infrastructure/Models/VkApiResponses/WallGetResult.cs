using System.Text.Json.Serialization;

namespace IzhevskEventRadar.Infrastructure.Models.VkApiResponses;

internal class WallGetResult
{
    [JsonPropertyName("items")]
    public IReadOnlyCollection<WallPostItem>? Items { get; set; }
}
