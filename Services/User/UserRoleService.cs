using LMS.Data;
using LMS.DTOs.TableView;
using LMS.DTOs.User;
using LMS.Data.Entities.User;
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

    public async Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var userRole = await db.UsersRole.FirstOrDefaultAsync(a => a.Uuid == uuid);

        if (userRole == null)
            return DeleteUserResult.UserNotFound(uuid);

        var usersHasRole = await db.Users.AnyAsync(a => a.Role == userRole.Uuid);

        if (usersHasRole)
            return DeleteUserResult.UnknownError(uuid, "Нельзя удалить роль, так как у пользователей есть данная роль");
        
        try
        {
            db.UsersRole.Remove(userRole);
            await db.SaveChangesAsync();
        }
        catch (Exception errorMessage) { return DeleteUserResult.UnknownError(uuid, errorMessage.ToString()); }
        
        return DeleteUserResult.Success(uuid);
    }

    public async Task<CreateUserResult> CreateAsync(UserRoleDto model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        if (await db.UsersRole.AnyAsync(a => a.Name == model.Name))
            return CreateUserResult.UnknownError("Роль с таким именем уже существует");

        var usersRole = new UserRole()
        {
            Name = model.Name
        };
        try
        {
            db.UsersRole.Add(usersRole);
            await db.SaveChangesAsync();
        }
        catch (Exception errorMessage) { return CreateUserResult.UnknownError(errorMessage.ToString()); }
        
        return CreateUserResult.Success(usersRole.Uuid);
    }

    public async Task<UpdateUserResult> UpdateAsync(UserRoleDto model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var usersRole = await db.UsersRole.FirstOrDefaultAsync(a => a.Uuid == model.Uuid);

        if (usersRole == null)
            return UpdateUserResult.UserNotFound(model.Uuid);

        usersRole.Name = model.Name;
        
        try
        {
            await db.SaveChangesAsync();
            return UpdateUserResult.Success(model.Uuid);
        }
        catch (Exception errorMessage) { return UpdateUserResult.UnknownError(model.Uuid, errorMessage.ToString()); }
    }
}