namespace LMS.DTOs.Director;

public class DirectorListItemVm
{
    public Guid Uuid { get; init; }
    public string Post { get; init; }
    public string Surname { get; init; }
    public string Name { get; init; }
    public string? Patronymic { get; init; }
    public DateTime CreatedAt { get; init; }
    
    public string? OrganizationsNames { get; init; } = "";
    public List<Guid>? OrganizationsUuids { get; init; }
    public string? BranchesNames { get; init; } = "";
    public List<Guid>? BranchesUuids { get; init; }
}