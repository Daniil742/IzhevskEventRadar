namespace IzhevskEventRadar.Contracts.Models;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Organization { get; set; }
    public DateOnly Date { get; set; }
    public string? Time { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
