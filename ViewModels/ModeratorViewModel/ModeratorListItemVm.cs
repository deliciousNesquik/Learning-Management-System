namespace LMS.ViewModels.ModeratorViewModel;

public class ModeratorListItemVm
{
    public Guid Uuid { get; init; }
    public string Login { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Surname { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Patronymic { get; set; }
    
    // Специфичные поля для модератора
    public string OrganizationName { get; init; } = "";
    public Guid OrganizationUuid { get; init; }
    public string BranchesNames { get; init; } = "";
    public List<Guid> BranchesUuids { get; init; }
}