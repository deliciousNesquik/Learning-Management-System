using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("courses", Schema = "public")]
public class Course
{
    [Key] 
    [Column("uuid")] 
    public Guid Uuid { get; init; }
    
    [Column("category_uuid")] 
    public Guid CategoryUuid { get; set; }
    
    [Column("author_uuid")] 
    public Guid AuthorUuid { get; set; }
    
    [Column("name")] 
    public string Name { get; set; } = string.Empty;
    
    [Column("description")] 
    public string Description { get; set; } = string.Empty;

    [Column("status")] 
    public string Status { get; set; } = "draft";
    
    [Column("estimated_duration_minutes")] 
    public int Duration { get; set; } = 0;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CategoryUuid))]
    public CoursesCategories CoursesCategories { get; set; } = null!;
    
    [ForeignKey(nameof(AuthorUuid))]
    public Administrator Administrators { get; set; } = null!;
}