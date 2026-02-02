using LMS.DTOs.TableView;
using LMS.DTOs.User;
using LMS.Interfaces.User;
using LMS.Models.UserManipulationResult;

namespace LMS.Services.User;

public class UserService: IUserServiceTest
{
    public Task<PagedResult<UserDto>> GetPagedResult(TableQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        throw new NotImplementedException();
    }

    public Task<CreateUserResult> CreateAsync(UserDto model)
    {
        throw new NotImplementedException();
    }

    public Task<UpdateUserResult> UpdateAsync(UserDto model)
    {
        throw new NotImplementedException();
    }
}