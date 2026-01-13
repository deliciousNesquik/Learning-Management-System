using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Data.Models;

namespace LMS.Data.Models;
[Table("employees", Schema = "public")]
public class Employee
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("login")]
    public string Login { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("is_active")] public bool IsActive { get; set; } = false;
    
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("surname")]
    public string Surname { get; set; } = null!;

    [Column("name")]
    public string Name { get; set; } = null!;
    
    [Column("patronymic")]
    public string? Patronymic { get; set; } = null!;
    
    [Column("post")]
    public string Post { get; set; } = null!;
    
    [Column("affiliation")]
    public string Affiliation { get; set; } = null!;
    
    [Column("insurance")]
    public string Insurance { get; set; } = null!;

    [Column("branch_uuid")]
    public Guid BranchUuid { get; set; }

    [Column("assigned_moderator_uuid")]
    public Guid AssignedModeratorUuid { get; set; }

    [ForeignKey(nameof(BranchUuid))]
    public virtual Organization Branches { get; set; } = null!;

    [ForeignKey(nameof(AssignedModeratorUuid))]
    public virtual Moderator AssignedModerator { get; set; } = null!;
}