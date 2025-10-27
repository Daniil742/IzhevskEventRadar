using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.DataBase.Context;
using IzhevskEventRadar.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IzhevskEventRadar.Infrastructure.Services.Internal;

internal class EventRepository(
    IDbContextFactory<EventRadarDbContext> dbContextFactory
    ) : IEventRepository
{
    private readonly IDbContextFactory<EventRadarDbContext> _dbContextFactory = dbContextFactory;

    public async Task<IReadOnlyCollection<EventDto>> GetEventsByDate(DateOnly date, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        //var startDate = DateOnly.ToDateTime(TimeOnly.MinValue);
        //var endDate = DateOnly.ToDateTime(TimeOnly.MaxValue);

        var query = from e in context.Events.AsNoTracking()

                    where
                        e.EventDate >= date &&
                        e.EventDate <= date

                    select new EventDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Organization = e.Organization,
                        Date = e.EventDate,
                        Time = e.Time,
                        Location = e.Location,
                        Latitude = e.Latitude,
                        Longitude = e.Longitude
                    };

        return await query.ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<EventDto>> GetEventsByDateRange(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

        var query = from e in context.Events.AsNoTracking()

                    where
                        e.EventDate >= startDate &&
                        e.EventDate <= endDate

                    select new EventDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Organization = e.Organization,
                        Date = e.EventDate,
                        Time = e.Time,
                        Location = e.Location,
                        Latitude = e.Latitude,
                        Longitude = e.Longitude
                    };

        return await query.ToArrayAsync(cancellationToken);
    }
}
