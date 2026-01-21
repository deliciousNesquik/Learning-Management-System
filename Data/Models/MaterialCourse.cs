using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("materials_courses_list", Schema = "public")]
public class MaterialCourse
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required]
    [Column("material_uuid")]
    public Guid MaterialUuid { get; set; }
        
    [Required]
    [Column("course_uuid")]
    public Guid CourseUuid { get; set; }
        
    // Навигационные свойства
    public Material Material { get; set; } = null!;
    public Course Course { get; set; } = null!;
}