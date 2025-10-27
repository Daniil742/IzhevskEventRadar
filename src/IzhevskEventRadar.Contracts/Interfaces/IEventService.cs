using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Contracts.Interfaces;

public interface IEventService
{
    Task<IReadOnlyCollection<EventDto>> GetEventsByDate(DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventDto>> GetEventsByDateRange(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
}
