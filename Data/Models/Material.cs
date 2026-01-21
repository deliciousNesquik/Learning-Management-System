using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Models;

public class Material
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(255)]
    public string BucketName { get; set; } = string.Empty;
        
    [Required]
    public Guid TypeUuid { get; set; }
        
    // Навигационные свойства
    public MaterialType Type { get; set; } = null!;
    public ICollection<MaterialCourse> CourseMaterials { get; set; } = new List<MaterialCourse>();
}