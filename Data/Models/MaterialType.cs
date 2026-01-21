using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Models;

public class MaterialType
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
        
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}