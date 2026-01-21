using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Models;

public class AssessmentCourse
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required]
    public Guid AssessmentsUuid { get; set; }
        
    [Required]
    public Guid CoursesUuid { get; set; }
        
    // Навигационные свойства
    public Assessment Assessment { get; set; } = null!;
    public Course Course { get; set; } = null!;
}