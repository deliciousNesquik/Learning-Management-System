using LMS.Components.Table;
using LMS.Models.UserManipulationResult;
using LMS.ViewModels.AdminViewModel;

namespace LMS.Interfaces;

public interface IAdminService : IUserService<AdminListItemVm, AdminStatsVm, CreateAdminVm, EditAdminVm>
{
    Task<List<AdminListItemVm>> GetAllAsync();
}