namespace LMS.ViewModels.BranchViewModel;

public class BranchNamesLookupVm(Guid uuid, string name)
{
    public Guid Uuid { get; set; } = uuid;
    public string Name { get; set; } = name;
}