using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data.Models;

[Table("branches_directors", Schema = "public")]
[PrimaryKey(nameof(BranchUuid), nameof(DirectorsUuid))]
public class BranchesDirectors
{
    [Column("branch_uuid")]
    public Guid BranchUuid { get; set; }
    
    [Column("director_uuid")]
    public Guid DirectorsUuid { get; set; }
    
    [ForeignKey(nameof(BranchUuid))] public virtual Branch Branch { get; set; } = null!;
    [ForeignKey(nameof(DirectorsUuid))] public virtual Director Director { get; set; } = null!;
}