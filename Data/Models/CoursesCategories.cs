using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("courses_categories", Schema = "public")]
public class CoursesCategories
{
    [Key] 
    [Column("uuid")] 
    public Guid Uuid { get; init; }
    
    [Column("created_by")] 
    public Guid CreatedByUuid { get; set; }
    
    [Column("name")] 
    public string Name { get; set; } = string.Empty;
    
    [Column("description")] 
    public string Description { get; set; } = string.Empty;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [ForeignKey(nameof(CreatedByUuid))]
    public Administrator Administrators { get; set; } = null!;
}