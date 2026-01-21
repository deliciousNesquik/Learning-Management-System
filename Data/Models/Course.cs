using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("courses", Schema = "public")]
public class Course
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;
        
    [Required]
    public Guid CategoryUuid { get; set; }
        
    public string? Description { get; set; }
        
    [Required, MaxLength(50)]
    public string Status { get; set; } = "draft";
        
    [Required]
    public Guid AuthorUuid { get; set; }
        
    public int? EstimatedDurationMinutes { get; set; }
        
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    // Навигационные свойства
    public CourseCategory Category { get; set; } = null!;
    public Administrator Author { get; set; } = null!;
    public ICollection<MaterialCourse> Materials { get; set; } = new List<MaterialCourse>();
    public ICollection<AssessmentCourse> Assessments { get; set; } = new List<AssessmentCourse>();
}