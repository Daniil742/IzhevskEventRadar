//using Grpc.Core;
//using IzhevskEventRadar.Contracts.Interfaces;
//using IzhevskEventRadar.Contracts.Models;
//using IzhevskEventRadar.GrpcContracts;
//using Microsoft.Extensions.Logging;
//using System.Globalization;

//namespace IzhevskEventRadar.Infrastructure.GrpcServices;

//internal class EventGrpcServiceImpl(
//    IEventService eventService,
//    ILogger<EventGrpcServiceImpl> logger
//    ) : EventGrpcService.EventGrpcServiceBase
//{
//    private readonly IEventService _eventService = eventService;
//    private readonly ILogger<EventGrpcServiceImpl> _logger = logger;

//    public override async Task<EventsByDateResponse> GetEventsByDate(EventsByDateRequest request, ServerCallContext context)
//    {
//        _logger.LogInformation("gRPC request for date: {Date}", request.Date);

//        if (!DateOnly.TryParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
//        {
//            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format. Use YYYY-MM-DD."));
//        }

//        // Вызываем ваш существующий сервис
//        var eventsDto = await _eventService.GetEventsByDate(dateOnly, context.CancellationToken);

//        var response = new EventsByDateResponse();
//        foreach (var ev in eventsDto)
//        {
//            response.Events.Add(new EventSummary
//            {
//                Id = ev.Id,
//                Title = ev.Title,
//                Time = ev.Date.ToString("HH:mm"), // Отправляем только время
//                Location = ev.Location ?? "Место не указано"
//            });
//        }
//        return response;
//    }

//    public override async Task<EventByIdResponse> GetEventById(EventByIdRequest request, ServerCallContext context)
//    {
//        _logger.LogInformation("gRPC request for event ID: {EventId}", request.EventId);

//        // TODO: Добавить метод GetEventByIdAsync в ваш IEventService
//        // var eventDetailsDto = await _eventService.GetEventByIdAsync(request.EventId, context.CancellationToken);

//        // --- ЗАГЛУШКА ---
//        var eventDetailsDto = new EventDto(request.EventId, $"Событие {request.EventId}", "Полное описание события...", DateTime.Now, "Какое-то место", 56.85, 53.21);
//        // --- КОНЕЦ ЗАГЛУШКИ ---


//        if (eventDetailsDto == null)
//        {
//            throw new RpcException(new Status(StatusCode.NotFound, $"Event with ID {request.EventId} not found."));
//        }

//        return new EventByIdResponse
//        {
//            Id = eventDetailsDto.Id,
//            Title = eventDetailsDto.Title,
//            Description = eventDetailsDto.Description ?? "",
//            Date = eventDetailsDto.Date.ToString("o"), // ISO 8601 формат
//            Location = eventDetailsDto.Location ?? "Место не указано",
//            Latitude = eventDetailsDto.Latitude,
//            Longitude = eventDetailsDto.Longitude
//        };
//    }
//}
