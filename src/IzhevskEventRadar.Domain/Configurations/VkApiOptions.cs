using System.ComponentModel.DataAnnotations;

namespace IzhevskEventRadar.Domain.Configurations;

public class VkApiOptions
{
    public const string ConfigurationSectionName = "App:VkApi";

    [Required]
    public required string ApiUrl { get; init; }

    [Required]
    public required string ApiVersion { get; init; }
    
    [Required]
    public required string AccessToken { get; init; }
}
