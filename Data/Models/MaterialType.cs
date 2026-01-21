using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("materials_types", Schema = "public")]
public class MaterialType
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
        
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}