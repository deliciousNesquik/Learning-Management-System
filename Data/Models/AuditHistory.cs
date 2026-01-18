using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("audit_history", Schema = "public")]
public class AuditHistory
{
    [Key] 
    [Column("uuid")]
    public Guid Uuid { get; init; }
    
    [Column("table_name")] 
    public string TableName { get; init; }
    
    [Column("record_uuid")] 
    public Guid RecordUuid { get; init; }
    
    [Column("action")] 
    public string Action { get; init; }
    
    [Column("old_data")] 
    public string OldData { get; init; }
    
    [Column("new_data")] 
    public string NewData { get; init; }
    
    [Column("changed_at")] 
    public DateTime ChangedAt { get; init; }
    
    [Column("changed_by")] 
    public Guid? ChangedBy { get; init; }
}