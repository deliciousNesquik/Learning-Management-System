using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("legal_forms", Schema = "public")]
public class LegalForm
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;
    
    [Column("description")]
    public string Description { get; set; } = null!;
}