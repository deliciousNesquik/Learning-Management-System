using LMS.DTOs.TableView;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces;

public interface IUserService<TItemVm, TStatsVm, TCreateVm, TEditVm>
{
    
    
    
    
    Task<PagedResult<TItemVm>> GetPagedAsync(TableQuery query);
    Task<TStatsVm> GetStatsAsync();
    Task<CreateUserResult> CreateAsync(TCreateVm model);
    Task<UpdateUserResult> UpdateAsync(TEditVm model);
    Task<ResetUserPasswordResult> ResetPasswordAsync(Guid userUuid, string newPassword);
    Task<DeleteUserResult> DeleteAsync(Guid userUuid);
}