using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IzhevskEventRadar.DataBase.DataModels;

[Table("Events")]
public class EventDataModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateOnly EventDate { get; set; }

    public string? Time { get; set; }

    public string? Organization { get; set; }

    public string? Location { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
}
