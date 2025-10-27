using IzhevskEventRadar.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IzhevskEventRadar.Api.Endpoints
{
    public static partial class Endpoints
    {
        public static IEndpointRouteBuilder RegisterEventsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var mapGroup = endpoints.MapGroup("/api/events");

            mapGroup.MapGet("", async (
                [FromQuery] DateOnly date,
                [FromServices] IEventService eventService,
                CancellationToken cancellationToken) =>
            {
                var events = await eventService.GetEventsByDate(date, cancellationToken);
                return Results.Ok(events);
            });

            mapGroup.MapGet("/range", async (
                [FromQuery] DateOnly startDate,
                [FromQuery] DateOnly endDate,
                [FromServices] IEventService eventService,
                CancellationToken cancellationToken) =>
            {
                var events = await eventService.GetEventsByDateRange(startDate, endDate, cancellationToken);
                return Results.Ok(events);
            });

            return endpoints;
        }
    }
}
