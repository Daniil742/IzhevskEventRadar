using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Contracts.Commands;

public record StoreEventCommand(EventDataDto? EventData, string InternalGroupId);
