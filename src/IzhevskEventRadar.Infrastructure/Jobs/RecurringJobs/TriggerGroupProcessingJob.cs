using Hangfire;
using IzhevskEventRadar.Contracts.Commands;
using IzhevskEventRadar.Contracts.Interfaces.Jobs;
using IzhevskEventRadar.Domain.Configurations;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace IzhevskEventRadar.Infrastructure.Jobs.RecurringJobs;

public class TriggerGroupProcessingJob(
    IOptions<HangfireQueuesConfiguration> queues,
    IOptions<ScheduledJobConfiguration> scheduledJobSettings,
    IServiceProvider serviceProvider
    ) : IRecurringJob
{
    private const string JOB_ID = "trigger-group-processing-job";

    private readonly HangfireQueuesConfiguration _queues = queues.Value;
    private readonly ScheduledJobConfiguration _scheduledJobSettings = scheduledJobSettings.Value;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void Register(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<TriggerGroupProcessingJob>(
            JOB_ID,
            _queues.RecurringJobQueue.Name,
            x => x.Execute(),
            _scheduledJobSettings.TriggerGroupProcessingJob);
    }

    public async Task Execute()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TriggerGroupProcessingJob>>();

            var retryPolicy = Policy
                .Handle<ObjectDisposedException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, attempt, context) =>
                    {
                        logger.LogWarning(exception, "Publish failed due to ObjectDisposedException. Retrying in {TimeSpan}. Attempt {Attempt}", timeSpan, attempt);
                    });

            try
            {
                await publishEndpoint.Publish(new ProcessAllGroupsCommand());
                //await retryPolicy.ExecuteAsync(async () => {
                //    await publishEndpoint.Publish(new ProcessAllGroupsCommand());
                //});

                logger.LogInformation("Successfully published ProcessAllGroupsCommand after retries (if any).");

            }
            catch (ObjectDisposedException ex)
            {
                logger.LogError(ex, "Publish failed permanently after retries due to ObjectDisposedException.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred during publish.");
                throw;
            }
        }
    }
}
