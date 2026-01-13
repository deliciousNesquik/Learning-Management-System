namespace LMS.ViewModels.DirectorViewModel;

public class DirectorListItemVm
{
    public Guid Uuid { get; init; }
    public string Post { get; init; }
    public string Surname { get; init; }
    public string Name { get; init; }
    public string? Patronymic { get; init; }
    
    public string? OrganizationName { get; init; } = "";
    public Guid? OrganizationUuid { get; init; }
    public string BranchesNames { get; init; } = "";
    public List<Guid> BranchesUuids { get; init; }
}