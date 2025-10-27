namespace IzhevskEventRadar.Domain.Configurations;

public class ScheduledJobConfiguration
{
    public const string ConfigurationSectionName = "JobSchedule";

    public required string TriggerGroupProcessingJob { get; set; }
}
