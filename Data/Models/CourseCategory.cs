using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Models;

public class CourseCategory
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;
        
    public string? Description { get; set; }
        
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    [Required]
    public Guid CreatedBy { get; set; }
        
    // Навигационные свойства
    public Administrator Creator { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}