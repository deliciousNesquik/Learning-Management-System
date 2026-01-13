using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("moderators", Schema = "public")]
public class Moderator
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();

    [Column("login")]
    public string Login { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("surname")]
    public string Surname { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("patronymic")]
    public string? Patronymic { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}