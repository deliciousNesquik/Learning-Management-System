using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("assessments_courses_list", Schema = "public")]
public class AssessmentCourse
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required]
    [Column("assessment_uuid")]
    public Guid AssessmentUuid { get; set; }
        
    [Required]
    [Column("course_uuid")]
    public Guid CourseUuid { get; set; }
        
    // Навигационные свойства
    public Assessment Assessment { get; set; } = null!;
    public Course Course { get; set; } = null!;
}