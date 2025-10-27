namespace IzhevskEventRadar.Domain.Configurations;

public class RabbitMQOptions
{
    public const string ConfigurationSectionName = "RabbitMQ";

    public string Exchange { get; set; } = "main_exchange";

    public string RoutingKey { get; set; } = "main_routing_key";

    public required Uri Uri { get; set; }

    public required string Host { get; set; }
}
