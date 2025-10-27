namespace IzhevskEventRadar.Domain.Configurations;

public class HangfireQueuesConfiguration
{
    public const string ConfigurationSectionName = "HangfireSettings:Queues";

    public required HangfireQueueBaseModel RecurringJobQueue { get; set; }
}

public class HangfireQueueBaseModel
{
    public required string Name { get; set; }
}