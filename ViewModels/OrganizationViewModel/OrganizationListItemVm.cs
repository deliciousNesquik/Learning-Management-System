namespace LMS.ViewModels.OrganizationViewModel;

public class OrganizationListItemVm
{
    public Guid Uuid { get; init; }
    public string Name { get; init; } = "";
    public string LegalFormName { get; init; } = ""; // Например: ООО, ИП
    public Guid LegalFormUuid { get; init; }
    public long Taxpayer { get; init; } // ИНН
    public string? Mail { get; init; }  
    public string Contacts { get; init; } 
    
    public string? Region { get; init; }  
    public string? City { get; init; }  
    public string? Street { get; init; }  
    public string? HouseNumber { get; init; }  
    
    public DateTime CreatedAt { get; init; }
    
    public int CountBranches { get; init; }
}