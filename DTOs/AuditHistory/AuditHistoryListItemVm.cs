using System.Text.Json;

namespace LMS.DTOs.AuditHistory;

public class AuditHistoryListItemVm
{
    public Guid Uuid { get; init; }
    public string TableName { get; init; }
    public Guid RecordUuid { get; init; }
    public string Action { get; init; }
    public JsonDocument? OldData { get; init; }
    public JsonDocument? NewData { get; init; }
    public DateTime ChangedAt { get; init; }
    public Guid? ChangedBy { get; init; }
}