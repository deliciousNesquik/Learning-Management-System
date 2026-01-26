using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs.Branch;
using LMS.DTOs.TableView;
using LMS.Interfaces;
using LMS.Models.UserManipulationResult;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class BranchService(IDbContextFactory<DatabaseContext> dbFactory) : IBranchService
{
    public async Task<List<BranchNamesLookupVm>> GetAllBranchesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        return await db.Branches
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new BranchNamesLookupVm(l.Uuid, l.Name))
            .ToListAsync();
    }

    public async Task<PagedResult<BranchListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        IQueryable<Branch> q = db.Branches.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {

            q = q.Where(d =>
                d.Uuid.ToString().Contains(query.Search) ||
                d.Name.Contains(query.Search) ||
                d.OrganizationUuid.ToString().Contains(query.Search) ||
                //d.Organization.Name.ToString().Contains(query.Search) ||
                d.BranchCode.Contains(query.Search) 
                //|| (d.Region + " " + d.City + " " + d.Street + " " + d.HouseNumber).Contains(query.Search)
            );
        }

        q = (query.SortBy, query.SortDesc) switch
        {
            ("is_default", false) => q.OrderBy(d => d.IsDefault),
            ("is_default", true) => q.OrderByDescending(d => d.IsDefault),
            
            ("status", false) => q.OrderBy(d => d.Status),
            ("status", true) => q.OrderByDescending(d => d.Status),
            
            ("created_at", false) => q.OrderBy(d => d.CreatedAt),
            ("created_at", true) => q.OrderByDescending(d => d.CreatedAt),
            
            _ => q.OrderByDescending(d => d.CreatedAt)
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new BranchListItemVm
            {
                Uuid = d.Uuid,
                Name = d.Name,
                OrganizationUuid = d.OrganizationUuid,
                //OrganizationUuid = d.Organization.Uuid,
                OrganizationName = d.Organization.Name,
                IsDefault = d.IsDefault,
                BranchCode = d.BranchCode,
                Status = d.Status,
                Region = d.Region,
                City = d.City,
                Street = d.Street,
                HouseNumber = d.HouseNumber,
                Timezone = d.Timezone,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<BranchListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<CreateUserResult> CreateAsync(CreateBranchVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var branchCode = model.BranchCode.Trim();

        if (!await db.Organizations.AnyAsync(o => o.Uuid == model.OrganizationUuid))
            return CreateUserResult.UnknownError("Указанная организация не найдена");

        if (await db.Branches.AnyAsync(b => b.BranchCode == branchCode))
            return CreateUserResult.UnknownError("Филиал с таким кодом уже существует");

        var branch = new Branch
        {
            Name = model.Name.Trim(),
            IsDefault = model.IsDefault,
            BranchCode = branchCode,
            Status = true,
            Region = model.Region?.Trim(),
            City = model.City?.Trim(),
            Street = model.Street?.Trim(),
            HouseNumber = model.HouseNumber?.Trim(),
            OrganizationUuid = model.OrganizationUuid,
            Timezone = model.Timezone
        };

        try
        {
            db.Branches.Add(branch);
            await db.SaveChangesAsync();
            return CreateUserResult.Success(branch.Uuid);
        }
        catch (DbUpdateException ex)
        {
            return CreateUserResult.UnknownError(ex.Message);
        }
    }

    public async Task<UpdateUserResult> UpdateAsync(EditBranchVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        var branch = await db.Branches.FirstOrDefaultAsync(m => m.Uuid == model.Uuid);
        if (branch == null)
            return UpdateUserResult.UserNotFound(model.Uuid);

        // Проверка уникальности кода при переименовании (исключая текущий филиал)
        if (await db.Branches.AnyAsync(b => b.BranchCode == model.BranchCode.Trim() && b.Uuid != model.Uuid))
            return UpdateUserResult.UnknownError(model.Uuid, "Этот код филиала уже занят другим подразделением");

        try
        {
            branch.Name = model.Name.Trim();
            branch.BranchCode = model.BranchCode.Trim();
            branch.IsDefault = model.IsDefault;
            branch.Status = model.Status;
            branch.Region = model.Region?.Trim();
            branch.City = model.City?.Trim();
            branch.Street = model.Street?.Trim();
            branch.HouseNumber = model.HouseNumber?.Trim();
            branch.OrganizationUuid = model.OrganizationUuid;
            branch.Timezone = model.Timezone; // Добавлено отсутствующее поле
            
            await db.SaveChangesAsync();
            return UpdateUserResult.Success(model.Uuid);
        }
        catch (DbUpdateException ex)
        {
            return UpdateUserResult.UnknownError(model.Uuid, ex.Message);
        }
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        // Добавим проверку: нельзя удалить филиал, если к нему привязаны пользователи (директора/модераторы)
        // Если в БД не настроено каскадное удаление, это предотвратит ошибку foreign key
        var hasLinks = await db.Set<BranchesDirectors>().AnyAsync(bd => bd.BranchUuid == uuid) ||
                       await db.Set<BranchesModerators>().AnyAsync(bm => bm.BranchUuid == uuid);
        
        if (hasLinks)
            return DeleteUserResult.UnknownError(uuid, "Нельзя удалить филиал, к которому привязаны сотрудники");

        var branch = await db.Branches.FirstOrDefaultAsync(m => m.Uuid == uuid);
        if (branch == null)
            return DeleteUserResult.UserNotFound(uuid);

        try
        {
            db.Branches.Remove(branch);
            await db.SaveChangesAsync();
            return DeleteUserResult.Success(uuid);
        }
        catch (DbUpdateException ex)
        { 
            return DeleteUserResult.UnknownError(uuid, ex.Message); 
        }
    }
}