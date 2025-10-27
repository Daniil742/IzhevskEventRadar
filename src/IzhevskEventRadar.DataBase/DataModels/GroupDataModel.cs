using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IzhevskEventRadar.DataBase.DataModels;

[Table("Groups")]
public class GroupDataModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("InternalId")]
    public string InternalId { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; } = false;
}
