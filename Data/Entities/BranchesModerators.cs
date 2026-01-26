using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data.Entities;

[Table("branches_moderators", Schema = "public")]
[PrimaryKey(nameof(BranchUuid), nameof(ModeratorUuid))]
public class BranchesModerators
{
    [Column("branch_uuid")]
    public Guid BranchUuid { get; set; }
    
    [Column("moderator_uuid")]
    public Guid ModeratorUuid { get; set; }
    
    [ForeignKey(nameof(BranchUuid))] public virtual Branch Branch { get; set; } = null!;
    [ForeignKey(nameof(ModeratorUuid))] public virtual Moderator Moderator { get; set; } = null!;
}