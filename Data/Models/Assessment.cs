using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("assessments", Schema = "public")]
public class Assessment
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required]
    [Column("name")]
    public int Name { get; set; } // Возможно, это опечатка и должно быть string
        
    [Required]
    [Column("time_limit")]
    public int TimeLimit { get; set; }
        
    [Required]
    [Column("attempts")]
    public int Attempts { get; set; }
        
    [Required]
    [Column("pass_score")]
    public int PassScore { get; set; }
        
    [Column("shuffle")]
    public bool? Shuffle { get; set; }
        
    [Required]
    [Column("question_limit")]
    public int QuestionLimit { get; set; }
        
    public ICollection<AssessmentCourse> CourseAssessments { get; set; } = new List<AssessmentCourse>();
}