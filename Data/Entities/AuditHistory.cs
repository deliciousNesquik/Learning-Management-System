using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LMS.Data.Entities;

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
    
    [Column("old_data",  TypeName = "jsonb")] 
    public JsonDocument? OldData { get; init; }
    
    [Column("new_data",  TypeName = "jsonb")] 
    public JsonDocument? NewData { get; init; }
    
    [Column("changed_at")] 
    public DateTime ChangedAt { get; init; }
    
    [Column("changed_by")] 
    public Guid? ChangedBy { get; init; }
}