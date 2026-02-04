using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs.TableView;
using LMS.DTOs.User;
using LMS.Interfaces.User;
using LMS.Models.UserManipulationResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services.User;

public class UserService (IDbContextFactory<DatabaseContext> dbFactory, IPasswordHasher<Data.Entities.User.User> passwordHasher): IUserServiceTest
{
    public Task<PagedResult<UserDto>> GetPagedResult(TableQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
        if (user == null)
            return DeleteUserResult.UserNotFound(uuid);

        try
        {
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return DeleteUserResult.Success(uuid);
        }
        catch (Exception ex)
        {
            return DeleteUserResult.UnknownError(uuid, ex.ToString());
        }
    }

    public async Task<CreateUserResult> CreateAsync(UserDto model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        if (await db.Users.AnyAsync(a => a.Login == model.Login))
            return CreateUserResult.LoginAlreadyExists();

        var user = new Data.Entities.User.User
        {
            Login = model.Login,
            Role = model.Role,
            IsActive = model.IsActive,
            Surname = model.Surname,
            Name = model.Name,
            GivenName = model.GivenName,
            AdditionalFields = model.AdditionalFields,
            CreatedAt = model.CreatedAt,
            CreatedBy = model.CreatedBy,
        };
        try
        {
            user.Password = passwordHasher.HashPassword(user, model.PlainPassword);
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        catch (Exception errorMessage) { return CreateUserResult.UnknownError(errorMessage.ToString()); }
        
        return CreateUserResult.Success(user.Uuid);
    }

    public async Task<UpdateUserResult> UpdateAsync(UserDto model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Uuid == model.Uuid);
        if (existingUser == null)
            return UpdateUserResult.UserNotFound(model.Uuid);

        // Проверка на дубликат логина (кроме текущего пользователя)
        if (await db.Users.AnyAsync(u => u.Login == model.Login && u.Uuid != model.Uuid))
            return UpdateUserResult.UnknownError(model.Uuid, "Login already exists");

        try
        {
            // Обновление свойств пользователя
            existingUser.Login = model.Login;
            existingUser.Role = model.Role;
            existingUser.IsActive = model.IsActive;
            existingUser.Surname = model.Surname;
            existingUser.Name = model.Name;
            existingUser.GivenName = model.GivenName;
            existingUser.AdditionalFields = model.AdditionalFields;

            // Обновление пароля только если передан новый
            if (!string.IsNullOrEmpty(model.PlainPassword))
            {
                existingUser.Password = passwordHasher.HashPassword(existingUser, model.PlainPassword);
            }

            await db.SaveChangesAsync();
            return UpdateUserResult.Success(existingUser.Uuid);
        }
        catch (Exception ex)
        {
            return UpdateUserResult.UnknownError(model.Uuid, ex.ToString());
        }
    }
}