using LMS.DTOs.Admin;

namespace LMS.Interfaces;

public interface IAdminService : IUserService<AdminListItemVm, AdminStatsVm, CreateAdminVm, EditAdminVm>
{
    Task<List<AdminListItemVm>> GetAllAsync();
}