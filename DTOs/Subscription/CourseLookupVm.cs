namespace LMS.DTOs.Subscription;

public class CourseLookupVm(Guid uuid, string name)
{
    public Guid Uuid { get; init; } = uuid;
    public string Name { get; init; } = name;
}