using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("courses", Schema = "public")]
public class Course
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
        
    [Required]
    [Column("category_uuid")]
    public Guid CategoryUuid { get; set; }
        
    [Column("description")]
    public string? Description { get; set; }
        
    [Required, MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "draft";
        
    [Required]
    [Column("author_uuid")]
    public Guid AuthorUuid { get; set; }
    
    [Column("estimated_duration_minutes")]
    public int? EstimatedDurationMinutes { get; set; }
        
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    // Навигационные свойства
    public CourseCategory Category { get; set; } = null!;
    public Administrator Author { get; set; } = null!;
    public ICollection<MaterialCourse> Materials { get; set; } = new List<MaterialCourse>();
    public ICollection<AssessmentCourse> Assessments { get; set; } = new List<AssessmentCourse>();
}