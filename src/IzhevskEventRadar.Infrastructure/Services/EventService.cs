using IzhevskEventRadar.Contracts.Interfaces;
using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.Infrastructure.Interfaces;

namespace IzhevskEventRadar.Infrastructure.Services;

internal class EventService(
    IEventRepository eventRepository
    ) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;

    public async Task<IReadOnlyCollection<EventDto>> GetEventsByDate(DateOnly date, CancellationToken cancellationToken = default)
    {
        var result = await _eventRepository.GetEventsByDate(date, cancellationToken);

        return result;
    }

    public async Task<IReadOnlyCollection<EventDto>> GetEventsByDateRange(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        var result = await _eventRepository.GetEventsByDateRange(startDate, endDate, cancellationToken);

        return result;
    }
}
