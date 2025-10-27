using System.Text.Json.Serialization;

namespace IzhevskEventRadar.Infrastructure.Models.VkApiResponses;

internal class PhotoAttachment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("orig_photo")]
    public OriginalPhoto? OriginalPhoto { get; set; }
}
