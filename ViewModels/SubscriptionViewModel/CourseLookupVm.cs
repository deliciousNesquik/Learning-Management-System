namespace LMS.ViewModels.SubscriptionViewModel;

public class CourseLookupVm(Guid uuid, string name)
{
    public Guid Uuid { get; init; } = uuid;
    public string Name { get; init; } = name;
}