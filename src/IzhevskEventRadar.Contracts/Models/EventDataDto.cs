namespace IzhevskEventRadar.Contracts.Models;

public record EventDataDto(
    string Title,
    string Date,
    string? Time,
    string? Location,
    double? Latitude,
    double? Longitude,
    string? Organization,
    string Description
);
