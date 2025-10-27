using System.Text.Json.Serialization;

namespace IzhevskEventRadar.Infrastructure.Models.VkApiResponses;

internal class WallPostItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("marked_as_ads")]
    public int MarkedAsAds { get; set; }

    [JsonPropertyName("date")]
    public long PublishDate { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("attachments")]
    public IReadOnlyCollection<WallPostAttachment>? WallPostAttachments { get; set; }
}
