namespace LMS.DTOs.Organization;

public record LegalFormLookupVm(Guid uuid, string name)
{
    public Guid? Uuid { get; set; } = uuid;
    public string Name { get; set; } = name;
}