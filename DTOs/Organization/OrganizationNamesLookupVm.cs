namespace LMS.DTOs.Organization;

public class OrganizationNamesLookupVm(Guid uuid, string name)
{
    public Guid Uuid { get; set; } = uuid;
    public string Name { get; set; } = name;
}