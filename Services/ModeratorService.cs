using LMS.Components.Table;
using LMS.Data;
using LMS.Data.Models;
using LMS.DTOs.Moderator;
using LMS.Interfaces;
using LMS.Models.UserManipulationResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class ModeratorService(
    IDbContextFactory<DatabaseContext> dbFactory,
    IPasswordHasher<Moderator> passwordHasher)
    : IModeratorService
{
    
    public async Task<PagedResult<ModeratorListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var q = db.Moderators
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            q = q.Where(m =>
                m.Uuid.ToString().Contains(query.Search) ||
                (m.Surname + " " + m.Name + " " + (m.Patronymic ?? "")).Contains(query.Search));
        }

        // --- СОРТИРОВКА ---
        q = (query.SortBy?.ToLower(), query.SortDesc) switch
        {
            
            ("is_active", false) => q.OrderBy(m => m.IsActive),
            ("is_active", true) => q.OrderByDescending(m => m.IsActive),

            ("created_at", false) => q.OrderBy(m => m.CreatedAt),
            ("created_at", true) => q.OrderByDescending(m => m.CreatedAt),

            _ => q.OrderByDescending(m => m.CreatedAt)
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(m => new ModeratorListItemVm
            {
                Uuid = m.Uuid,
                Login = m.Login,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                Surname = m.Surname,
                Name = m.Name,
                Patronymic = m.Patronymic,

                OrganizationName = string.Join(", ", db.Set<BranchesModerators>()
                    .Where(bd => bd.ModeratorUuid == m.Uuid)
                    .Select(bd => bd.Branch.Organization.Name)
                    .Distinct()),
                
                OrganizationUuid = db.BranchesModerators
                    .Where(bd => bd.ModeratorUuid == m.Uuid)
                    .Select(bd => bd.Branch.Organization.Uuid)
                    .Distinct()
                    .First(),
                
                BranchesNames = string.Join(", ", db.Set<BranchesModerators>()
                    .Where(bd => bd.ModeratorUuid == m.Uuid)
                    .Select(bd => bd.Branch.Name)),
                
                BranchesUuids = db.BranchesModerators
                    .Where(bd => bd.ModeratorUuid == m.Uuid)
                    .Select(bd => bd.Branch.Uuid)
                    .ToList()
            })
            .ToListAsync();

        return new PagedResult<ModeratorListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<ModeratorStatsVm> GetStatsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var total = await db.Moderators.CountAsync();
        var active = await db.Moderators.CountAsync(m => m.IsActive);
        
        // Считаем организации, у которых нет ни одного модератора
        var orgsWithoutMods = await db.Organizations
            .CountAsync(o =>
                !db.Branches.Any(b =>
                    b.OrganizationUuid == o.Uuid &&
                    db.BranchesModerators.Any(bm => bm.BranchUuid == b.Uuid)
                )
            );

        return new ModeratorStatsVm
        {
            Total = total,
            Active = active,
            Blocked = total - active,
            OrganizationsWithoutModerators = orgsWithoutMods
        };
    }

    public async Task<CreateUserResult> CreateAsync(CreateModeratorVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // Проверка глобальной уникальности логина
        if (await db.Moderators.AnyAsync(m => m.Login == model.Login))
            return CreateUserResult.LoginAlreadyExists();
        
        var moderator = new Moderator 
        { 
            Login = model.Login, 
            Surname = model.Surname,
            Name = model.Name,
            Patronymic = model.Patronymic,
            IsActive = true, 
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            moderator.Password = passwordHasher.HashPassword(moderator, model.PlainPassword);
            db.Moderators.Add(moderator);
            
            foreach (var branchUuid in model.BranchesUuids)
            {
                var link = new BranchesModerators()
                {
                    ModeratorUuid = moderator.Uuid,
                    BranchUuid = branchUuid
                };
                db.BranchesModerators.Add(link);
            }
            
            await db.SaveChangesAsync();
            return CreateUserResult.Success(moderator.Uuid);
        }
        catch (Exception ex) 
        { 
            return CreateUserResult.UnknownError(ex.Message); 
        }
    }

    public async Task<UpdateUserResult> UpdateAsync(EditModeratorVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var moderator = await db.Moderators.FirstOrDefaultAsync(m => m.Uuid == model.Uuid);

        if (moderator == null)
            return UpdateUserResult.UserNotFound(model.Uuid);
        
        try
        {
            moderator.Login = model.Login;
            moderator.Surname = model.Surname;
            moderator.Name = model.Name;
            moderator.Patronymic = model.Patronymic;
            moderator.IsActive = model.IsActive;
           
            // 3. Обновляем связи с филиалами
            // Сначала удаляем все текущие связи директора
            var currentLinks = db.Set<BranchesModerators>().Where(bd => bd.ModeratorUuid == moderator.Uuid);
            db.Set<BranchesModerators>().RemoveRange(currentLinks);

            // Затем добавляем новые
            foreach (var branchUuid in model.BranchesUuids)
            {
                db.Set<BranchesModerators>().Add(new BranchesModerators()
                {
                    ModeratorUuid = moderator.Uuid,
                    BranchUuid = branchUuid
                });
            }
            
            await db.SaveChangesAsync();
            return UpdateUserResult.Success(model.Uuid);
        }
        catch (Exception ex) 
        { 
            return UpdateUserResult.UnknownError(model.Uuid, ex.Message); 
        }
    }

    public async Task<ResetUserPasswordResult> ResetPasswordAsync(Guid userUuid, string newPassword)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var moderator = await db.Moderators.FirstOrDefaultAsync(m => m.Uuid == userUuid);

        if (moderator == null)
            return ResetUserPasswordResult.UserNotFound(userUuid);

        try
        {
            moderator.Password = passwordHasher.HashPassword(moderator, newPassword);
            await db.SaveChangesAsync();
            return ResetUserPasswordResult.Success(userUuid);
        }
        catch (Exception ex) 
        { 
            return ResetUserPasswordResult.UnknownError(userUuid, ex.Message); 
        }
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid userUuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var moderator = await db.Moderators.FirstOrDefaultAsync(m => m.Uuid == userUuid);

        if (moderator == null)
            return DeleteUserResult.UserNotFound(userUuid);

        try
        {
            var links = db.Set<BranchesModerators>().Where(bd => bd.ModeratorUuid == userUuid);
            db.Set<BranchesModerators>().RemoveRange(links);
            
            db.Moderators.Remove(moderator);
            await db.SaveChangesAsync();
            return DeleteUserResult.Success(userUuid);
        }
        catch (Exception ex) 
        { 
            return DeleteUserResult.UnknownError(userUuid, ex.Message); 
        }
    }

    public Task<List<ModeratorListItemVm>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}