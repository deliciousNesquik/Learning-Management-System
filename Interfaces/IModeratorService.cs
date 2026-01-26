using LMS.Components.Table;
using LMS.DTOs.Moderator;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces;

public interface IModeratorService : IUserService<ModeratorListItemVm, ModeratorStatsVm, CreateModeratorVm, EditModeratorVm>
{
    Task<List<ModeratorListItemVm>> GetAllAsync();
}