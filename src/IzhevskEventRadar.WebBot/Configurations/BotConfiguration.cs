using System.ComponentModel.DataAnnotations;

namespace IzhevskEventRadar.WebBot.Configurations;

public class BotConfiguration
{
    public const string ConfigurationSectionName = "BotConfiguration";

    [Required]
    public string BotToken { get; set; }
}
