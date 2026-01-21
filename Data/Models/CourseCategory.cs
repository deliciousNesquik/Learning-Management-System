using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("courses_categories", Schema = "public")]
public class CourseCategory
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("description")]
    public string? Description { get; set; }
        
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    [Required]
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
    
    [Required] 
    [Column("color_hex")] 
    public string ColorHex { get; set; } = "#828282";
        
    // Навигационные свойства
    public Administrator Creator { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}