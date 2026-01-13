namespace LMS.ViewModels.ModeratorViewModel;

public class ModeratorStatsVm
{
    public int Total { get; init; }
    public int Active { get; init; }
    public int Blocked { get; init; }
    
    // Поможет админу понять, где еще не настроено обучение
    public int OrganizationsWithoutModerators { get; init; }
}