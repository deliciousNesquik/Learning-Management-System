using LMS.DTOs.TableView;
using LMS.DTOs.User;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces.User;

public interface IUserServiceTest
{
    Task<PagedResult<UserDto>> GetPagedResult(TableQuery query);
    Task<DeleteUserResult> DeleteAsync(Guid uuid);
    Task<CreateUserResult> CreateAsync(UserDto model);
    Task<UpdateUserResult> UpdateAsync(UserDto model);
}