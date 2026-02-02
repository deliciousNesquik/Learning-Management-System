using LMS.Data;
using LMS.DTOs.TableView;
using LMS.DTOs.User;
using LMS.Interfaces.User;
using LMS.Models.UserManipulationResult;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services.User;

public class UserRoleService (IDbContextFactory<DatabaseContext> dbFactory): IUserRoleService
{
    public async Task<PagedResult<UserRoleDto>> GetPagedResult(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        var q = db.UsersRole.AsNoTracking();
        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new UserRoleDto()
            {
                Uuid = a.Uuid,
                Name = a.Name,
            })
            .ToListAsync();

        return new PagedResult<UserRoleDto>
        {
            Items = items,
            TotalCount = total
        };
    }

    public Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        throw new NotImplementedException();
    }

    public Task<CreateUserResult> CreateAsync(UserRoleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<UpdateUserResult> UpdateAsync(UserRoleDto model)
    {
        throw new NotImplementedException();
    }
}