using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Infrastructure.Interfaces;

internal interface IEventRepository
{
    Task<IReadOnlyCollection<EventDto>> GetEventsByDate(DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventDto>> GetEventsByDateRange(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
}
