using LMS.DTOs.Moderator;


namespace LMS.Interfaces;

public interface IModeratorService : IUserService<ModeratorListItemVm, ModeratorStatsVm, CreateModeratorVm, EditModeratorVm>
{
    Task<List<ModeratorListItemVm>> GetAllAsync();
}