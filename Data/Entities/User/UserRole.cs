using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("users_role", Schema = "public")]
public class UserRole
{
    [Key] [Column("uuid")] 
    public Guid Uuid { get; init; }
    
    [Column("name")] 
    public string Name { get; set; } = string.Empty;
}