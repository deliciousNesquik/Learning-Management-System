using LMS.Components.Table;
using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs.Admin;
using LMS.Interfaces;   
using LMS.Models.UserManipulationResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class AdminService(
    IDbContextFactory<DatabaseContext> dbFactory,
    IPasswordHasher<Administrator> passwordHasher)
    : IAdminService
{
    public async Task<List<AdminListItemVm>> GetAllAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.Administrators
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AdminListItemVm
            {
                Uuid = a.Uuid,
                Login = a.Login,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                Surname = a.Surname,
                Name = a.Name,
                Patronymic = a.Patronymic,
            })
            .ToListAsync();
    }

    public async Task<PagedResult<AdminListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var q = db.Administrators.AsNoTracking();

        // --- ПОИСК ---
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            q = q.Where(a =>
                a.Uuid.ToString().Contains(query.Search) ||
                a.Login.Contains(query.Search) ||
                (a.Surname + " " + a.Name + " " + (a.Patronymic ?? "")).Contains(query.Search)
                );
        }

        // --- СОРТИРОВКА ---
        q = (query.SortBy, query.SortDesc) switch
        {
            ("login", false) => q.OrderBy(a => a.Login),
            ("login", true) => q.OrderByDescending(a => a.Login),
            
            ("is_active", false) => q.OrderBy(a => a.IsActive),
            ("is_active", true) => q.OrderByDescending(a => a.IsActive),

            ("created_at", false) => q.OrderBy(a => a.CreatedAt),
            ("created_at", true) => q.OrderByDescending(a => a.CreatedAt),

            _ => q.OrderByDescending(a => a.CreatedAt)
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new AdminListItemVm
            {
                Uuid = a.Uuid,
                Login = a.Login,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                Surname = a.Surname,
                Name = a.Name,
                Patronymic = a.Patronymic,
            })
            .ToListAsync();

        return new PagedResult<AdminListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<AdminStatsVm> GetStatsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var total = await db.Administrators.CountAsync();
        var active = await db.Administrators.CountAsync(a => a.IsActive);

        return new AdminStatsVm
        {
            Total = total,
            Active = active,
            Blocked = total - active
        };
    }

    public async Task<CreateUserResult> CreateAsync(CreateAdminVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        if (await db.Administrators.AnyAsync(a => a.Login == model.Login))
            return CreateUserResult.LoginAlreadyExists();

        var admin = new Administrator
        {
            Login = model.Login, 
            IsActive = true,
            Surname = model.Surname,
            Name = model.Name,
            Patronymic = model.Patronymic,
        };
        try
        {
            admin.Password = passwordHasher.HashPassword(admin, model.PlainPassword);
            db.Administrators.Add(admin);
            await db.SaveChangesAsync();
        }
        catch (Exception errorMessage) { return CreateUserResult.UnknownError(errorMessage.ToString()); }
        
        return CreateUserResult.Success(admin.Uuid);
    }

    public async Task<UpdateUserResult> UpdateAsync(EditAdminVm model)
    {
        Console.WriteLine("начало");
        await using var db = await dbFactory.CreateDbContextAsync();
        var admin = await db.Administrators.FirstOrDefaultAsync(a => a.Uuid == model.Uuid);

        if (admin == null)
            return UpdateUserResult.UserNotFound(model.Uuid);

        admin.Login = model.Login;
        admin.IsActive = model.IsActive;
        admin.Surname = model.Surname;
        admin.Name = model.Name;
        admin.Patronymic = model.Patronymic;
        
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Выполнилось...");
            return UpdateUserResult.Success(model.Uuid);
        }
        catch (Exception errorMessage) { return UpdateUserResult.UnknownError(model.Uuid, errorMessage.ToString()); }
    }

    public async Task<ResetUserPasswordResult> ResetPasswordAsync(Guid adminUuid, string newPassword)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var admin = await db.Administrators.FirstOrDefaultAsync(a => a.Uuid == adminUuid);

        if (admin == null)
            return ResetUserPasswordResult.UserNotFound(adminUuid);

        if (!admin.IsActive)
            return ResetUserPasswordResult.UserInactive(adminUuid);

        try
        {
            admin.Password = passwordHasher.HashPassword(admin, newPassword);
            await db.SaveChangesAsync();
        }
        catch (Exception errorMessage) { return ResetUserPasswordResult.UnknownError(adminUuid, errorMessage.ToString()); }

        return ResetUserPasswordResult.Success(adminUuid);
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid adminUuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var admin = await db.Administrators.FirstOrDefaultAsync(a => a.Uuid == adminUuid);

        if (admin == null)
            return DeleteUserResult.UserNotFound(adminUuid);

        try
        {
            db.Administrators.Remove(admin);
            await db.SaveChangesAsync();
        }
        catch (Exception errorMessage) { return DeleteUserResult.UnknownError(adminUuid, errorMessage.ToString()); }
        
        return DeleteUserResult.Success(adminUuid);
    }

}