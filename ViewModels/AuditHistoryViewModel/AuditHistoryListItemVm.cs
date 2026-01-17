namespace LMS.ViewModels.AuditHistoryViewModel;

public class AuditHistoryListItemVm
{
    public Guid Uuid { get; init; }
    public string TableName { get; init; }
    public Guid RecordUuid { get; init; }
    public string Action { get; init; }
    public string? OldData { get; init; }
    public string? NewData { get; init; }
    public DateTime ChangedAt { get; init; }
    public Guid? ChangedBy { get; init; }
}