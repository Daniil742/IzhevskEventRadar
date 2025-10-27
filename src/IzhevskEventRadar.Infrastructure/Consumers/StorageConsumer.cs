using IzhevskEventRadar.Contracts.Commands;
using IzhevskEventRadar.DataBase.Context;
using IzhevskEventRadar.DataBase.DataModels;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace IzhevskEventRadar.Infrastructure.Consumers;

internal class StorageConsumer(
    IDbContextFactory<EventRadarDbContext> dbContextFactory
    ) : IConsumer<PostFetchedEvent>, IConsumer<StoreEventCommand>
{
    private readonly IDbContextFactory<EventRadarDbContext> _dbContextFactory = dbContextFactory;

    public async Task Consume(ConsumeContext<PostFetchedEvent> context)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(context.CancellationToken);

        var postData = context.Message.Post;

        if (postData is null)
            return;

        var entity = new PostDataModel
        {

        };

        await dbContext.Posts.AddAsync(entity, context.CancellationToken);

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<StoreEventCommand> context)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(context.CancellationToken);

        var eventData = context.Message.EventData;

        if (eventData is null)
            return;

        DateTime.TryParseExact(
            eventData.Date,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime date
        );

        var entity = new EventDataModel
        {
            Title = eventData.Title,
            EventDate = DateOnly.FromDateTime(date),
            Time = eventData.Time,
            Location = eventData.Location,
            Latitude = eventData.Latitude,
            Longitude = eventData.Longitude,
            Organization = eventData.Organization,
            Description = eventData.Description
        };

        await dbContext.Events.AddAsync(entity, context.CancellationToken);

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
