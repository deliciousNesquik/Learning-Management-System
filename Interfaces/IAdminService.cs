using LMS.Components.Table;
using LMS.DTOs.Admin;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces;

public interface IAdminService : IUserService<AdminListItemVm, AdminStatsVm, CreateAdminVm, EditAdminVm>
{
    Task<List<AdminListItemVm>> GetAllAsync();
}