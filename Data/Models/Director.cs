using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("directors", Schema = "public")]
public class Director
{
    [Key] 
    [Column("uuid")] 
    public Guid Uuid { get; init; }
    
    [Column("post")] 
    public string Post { get; set; } = string.Empty;
    
    [Column("created_at")] 
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    
    [Column("surname")] 
    public string Surname { get; set; } = string.Empty;
    
    [Column("name")] 
    public string Name { get; set; } = string.Empty;
    
    [Column("patronymic")] 
    public string? Patronymic { get; set; }
    
    public virtual ICollection<BranchesDirectors> BranchesDirectors { get; set; } = new List<BranchesDirectors>();
}