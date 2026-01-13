using LMS.Components.Table;
using LMS.Models.UserManipulationResult;
using LMS.ViewModels.ModeratorViewModel;

namespace LMS.Interfaces;

public interface IModeratorService : IUserService<ModeratorListItemVm, ModeratorStatsVm, CreateModeratorVm, EditModeratorVm>
{
    Task<List<ModeratorListItemVm>> GetAllAsync();
}