using Hangfire;
using Hangfire.PostgreSql;
using IzhevskEventRadar.Contracts.Interfaces.Jobs;
using IzhevskEventRadar.Domain.Configurations;
using IzhevskEventRadar.Infrastructure;
using IzhevskEventRadar.Infrastructure.Jobs.RecurringJobs;
using Microsoft.Extensions.Options;

namespace IzhevskEventRadar.Api.BuilderExtensions;

public static class HangfireBuilder
{
    public static void AddHangfireWithServer(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddHangfire((provider, options) =>
        {
            options.UsePostgreSqlStorage(postgresOpt =>
            {
                postgresOpt.UseNpgsqlConnection(configuration.GetConnectionString("EVENTRADARCONNECTIONSTRING")!);
            });

            options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            options.UseSimpleAssemblyNameTypeSerializer();
            options.UseRecommendedSerializerSettings();
            //options.UseFilterProvider(new HangfireFilterProvider(provider));
        });

        AddHangfireServers(serviceCollection, configuration);
        AddRecurringJobs(serviceCollection, configuration);
    }

    public static IApplicationBuilder UseHangfireSecureDashboard(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
        {
            DashboardTitle = "Hangfire Dashboard - Izhevsk Event Radar"
        });

        return app;
    }

    public static IApplicationBuilder InitializeRecurringJobs(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var recurringJobs = app.ApplicationServices.GetServices<IRecurringJob>();

        foreach (var job in recurringJobs)
            job.Register(recurringJobManager);

        return app;
    }

    private static void AddHangfireServers(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<HangfireQueuesConfiguration>()
            .Bind(configuration.GetSection("BrokerSettings:Queues"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddHangfireServer((serv, options) =>
        {
            var queueOptions = serv.GetRequiredService<IOptions<HangfireQueuesConfiguration>>().Value;

            options.ServerName = typeof(DependencyInjection).Namespace;
            options.Queues = new[]
            {
                queueOptions.RecurringJobQueue.Name
            };
            options.WorkerCount = 5;
        });
    }

    private static void AddRecurringJobs(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<ScheduledJobConfiguration>()
            .Bind(configuration.GetSection(ScheduledJobConfiguration.ConfigurationSectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddSingleton<IRecurringJob, TriggerGroupProcessingJob>();
    }
}
