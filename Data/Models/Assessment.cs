using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Models;

public class Assessment
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
        
    [Required]
    public int Name { get; set; } // Возможно, это опечатка и должно быть string
        
    [Required]
    public int TimeLimit { get; set; }
        
    [Required]
    public int Attempts { get; set; }
        
    [Required]
    public int PassScore { get; set; }
        
    public bool? Shuffle { get; set; }
        
    [Required]
    public int QuestionLimit { get; set; }
        
    public ICollection<AssessmentCourse> CourseAssessments { get; set; } = new List<AssessmentCourse>();
}