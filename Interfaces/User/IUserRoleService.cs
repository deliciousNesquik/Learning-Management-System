using LMS.DTOs.TableView;
using LMS.DTOs.User;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces.User;

public interface IUserRoleService
{ 
    Task<PagedResult<UserRoleDto>> GetPagedResult(TableQuery query);
    Task<DeleteUserResult> DeleteAsync(Guid uuid);
    Task<CreateUserResult> CreateAsync(UserRoleDto model);
    Task<UpdateUserResult> UpdateAsync(UserRoleDto model);
}