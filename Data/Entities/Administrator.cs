using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("administrators", Schema = "public")]
public class Administrator
{
    [Key] [Column("uuid")] 
    public Guid Uuid { get; init; }
    
    [Column("login")] 
    public string Login { get; set; } = string.Empty;

    [Column("password")] 
    public string Password { get; set; } = string.Empty;

    [Column("created_at")] 
    public DateTime CreatedAt { get; init; } = DateTime.Now;

    [Column("is_active")] 
    public bool IsActive { get; set; } = true;
    
    [Column("surname")] 
    public string Surname { get; set; } = string.Empty;
    
    [Column("name")] 
    public string Name { get; set; } = string.Empty;
    
    [Column("patronymic")] 
    public string? Patronymic { get; set; } = string.Empty;
    
    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<CourseCategory> CreatedCategories { get; set; } = new List<CourseCategory>();
}