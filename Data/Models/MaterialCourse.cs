using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Models;

public class MaterialCourse
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required]
    public Guid MaterialUuid { get; set; }
        
    [Required]
    public Guid CourseUuid { get; set; }
        
    // Навигационные свойства
    public Material Material { get; set; } = null!;
    public Course Course { get; set; } = null!;
}