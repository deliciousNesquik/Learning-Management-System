namespace LMS.ViewModels.BranchViewModel;

public class BranchListItemVm
{
    public Guid Uuid { get; init; }
    public string Name { get; init; }
    public bool IsDefault { get; set; }
    public Guid OrganizationUuid { get; init; }
    public string OrganizationName { get; set; }
    public string BranchCode { get; init; }
    public bool Status { get; init; }
    public string Region { get; init; }  
    public string City { get; init; }  
    public string Street { get; init; }  
    public string HouseNumber { get; init; }  
    public int Timezone { get; init; }
    public DateTime CreatedAt { get; init; }
}