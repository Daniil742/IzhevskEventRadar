using Hangfire;

namespace IzhevskEventRadar.Contracts.Interfaces.Jobs;

public interface IRecurringJob
{
    void Register(IRecurringJobManager recurringJobManager);
}
