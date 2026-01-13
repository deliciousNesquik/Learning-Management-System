namespace LMS.ViewModels;

public class TablePermissionVm
{
    public string TableName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty; // Понятное название (напр: "Организации")
    public bool CanSelect { get; set; }
    public bool CanInsert { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}