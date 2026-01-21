using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("materials", Schema = "public")]
public class Material
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(255)]
    [Column("bucket_name")]
    public string BucketName { get; set; } = string.Empty;
        
    [Required]
    [Column("type_uuid")]
    public Guid TypeUuid { get; set; }
        
    // Навигационные свойства
    public MaterialType Type { get; set; } = null!;
    public ICollection<MaterialCourse> CourseMaterials { get; set; } = new List<MaterialCourse>();
}