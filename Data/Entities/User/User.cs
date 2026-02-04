using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LMS.Data.Entities.User;

[Table("users", Schema = "public")]
public class User
{
    [Key] [Column("uuid")] 
    public Guid Uuid { get; init; }
    
    [Column("login")] 
    public string Login { get; set; } = string.Empty;

    [Column("password")] 
    public string Password { get; set; } = string.Empty;
    
    [Column("created_at")] 
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    
    [Column("created_by")] 
    public Guid CreatedBy { get; init; }
    
    [Column("surname")] 
    public string Surname { get; set; } = string.Empty;
    
    [Column("name")] 
    public string Name { get; set; } = string.Empty;
    
    [Column("given_name")] 
    public string? GivenName { get; set; } = string.Empty;
    
    [Column("role")]
    public Guid Role { get; set; }
    
    [Column("additional_fields")]
    public JsonContent? AdditionalFields { get; set; }
    
    [ForeignKey(nameof(Role))]
    public virtual UserRole UserRole { get; set; } = null!;
}