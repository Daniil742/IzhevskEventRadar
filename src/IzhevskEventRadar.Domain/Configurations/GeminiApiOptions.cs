using System.ComponentModel.DataAnnotations;

namespace IzhevskEventRadar.Domain.Configurations;

public class GeminiApiOptions
{
    public const string ConfigurationSectionName = "App:GeminiApi";

    [Required]
    public required string ApiKey { get; init; }

    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string Model { get; init; }
}
