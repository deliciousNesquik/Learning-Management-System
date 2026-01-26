using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("user_permissions", Schema = "public")]
public class UserPermission
{
    [Key, Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("user_uuid")]
    public Guid UserUuid { get; set; }

    [Column("table_name")]
    public string TableName { get; set; } = string.Empty;

    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [Column("can_select")]
    public bool CanSelect { get; set; }

    [Column("can_insert")]
    public bool CanInsert { get; set; }

    [Column("can_update")]
    public bool CanUpdate { get; set; }

    [Column("can_delete")]
    public bool CanDelete { get; set; }
}