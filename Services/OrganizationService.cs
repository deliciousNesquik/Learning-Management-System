using LMS.Components.Table;
using LMS.Data;
using LMS.Data.Models;
using LMS.Interfaces;
using LMS.Models.UserManipulationResult;
using LMS.ViewModels.BranchViewModel;
using LMS.ViewModels.OrganizationViewModel;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class OrganizationService(IDbContextFactory<DatabaseContext> dbFactory, IBranchService branchService) : IOrganizationService
{
    public async Task<PagedResult<OrganizationListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        IQueryable<Organization> baseQuery = db.Organizations
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            baseQuery = baseQuery.Where(o =>
                EF.Functions.Like(o.Name, $"%{search}%") ||
                o.Taxpayer.ToString().Contains(search));
            // предположение: Taxpayer не строка, иначе Like
        }

        var branchCounts = await db.Branches
            .GroupBy(b => b.OrganizationUuid)
            .Select(g => new { OrgId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OrgId, x => x.Count);
        
        var total = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => new
            {
                Org = o,
                BranchCount = db.Branches
                    .Where(b => b.OrganizationUuid == o.Uuid)
                    .Count()
            })
            .Select(x => new OrganizationListItemVm
            {
                Uuid = x.Org.Uuid,
                Name = x.Org.Name,
                LegalFormName = x.Org.LegalForm.Name,
                LegalFormUuid = x.Org.LegalForm.Uuid,
                Taxpayer = x.Org.Taxpayer,
                Mail = x.Org.Mail,
                Contacts = x.Org.Contacts,
                Region = x.Org.Region,
                City = x.Org.City,
                Street = x.Org.Street,
                HouseNumber = x.Org.HouseNumber,
                CreatedAt = x.Org.CreatedAt,
                CountBranches = branchCounts.GetValueOrDefault(x.Org.Uuid)
            })
            .ToListAsync();

        return new PagedResult<OrganizationListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<List<OrganizationNamesLookupVm>> GetAllOrganizationsNamesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        return await db.Organizations
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrganizationNamesLookupVm(o.Uuid, o.Name))
            .ToListAsync();
    }

    public async Task<OrganizationStatsVm> GetStatsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;

        var orgs = db.Organizations.AsNoTracking();

        return new OrganizationStatsVm
        {
            Total = await orgs.CountAsync(),
            NewThisMonth = await orgs.CountAsync(o =>
                o.CreatedAt.Year == now.Year &&
                o.CreatedAt.Month == now.Month),
            TotalModerators = await db.Moderators.AsNoTracking().CountAsync()
        };
    }

    public async Task<List<LegalFormLookupVm>> GetLegalFormsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.LegalForms
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LegalFormLookupVm(l.Uuid, l.Name))
            .ToListAsync();
    }

    public async Task<List<LegalFormLookupVm>> GetAllForLookupAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Organizations
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new LegalFormLookupVm(o.Uuid, o.Name)) // Используем ту же VM для простоты
            .ToListAsync();
    }
    
    private static Branch CreateDefaultBranch(Organization org)
    {
        return new Branch
        {
            Uuid = Guid.NewGuid(),
            Name = $"{org.Name} главный филиал",
            BranchCode = "BR-" + Guid.NewGuid().ToString("N")[..8],
            IsDefault = true,
            Status = true,
            OrganizationUuid = org.Uuid,
            Region = org.Region,
            City = org.City,
            Street = org.Street,
            HouseNumber = org.HouseNumber,
            Timezone = org.Timezone,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<CreateUserResult> CreateAsync(CreateOrganizationVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var org = new Organization
            {
                Uuid = Guid.NewGuid(),
                Name = model.Name,
                Taxpayer = model.TaxPayer,
                Region = model.Region,
                City = model.City,
                Street = model.Street,
                HouseNumber = model.HouseNumber,
                Mail = model.Mail,
                Contacts = model.Contacts,
                LegalFormUuid = model.LegalFormUuid!.Value,
                CreatedAt = DateTime.UtcNow
            };

            db.Organizations.Add(org);

            var defaultBranch = CreateDefaultBranch(org);
            db.Branches.Add(defaultBranch);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreateUserResult.Success(org.Uuid);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return CreateUserResult.UnknownError(ex.Message);
        }
    }


    public async Task<UpdateUserResult> UpdateAsync(EditOrganizationVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var org = await db.Organizations.FirstOrDefaultAsync(o => o.Uuid == model.Uuid);
        if (org == null) return UpdateUserResult.UserNotFound(model.Uuid);

        org.Name = model.Name;
        org.Taxpayer = model.TaxPayer;
        org.Region = model.Region;
        org.City = model.City;
        org.Street = model.Street;
        org.HouseNumber = model.HouseNumber;
        org.Mail = model.Mail;
        org.Contacts = model.Contacts;
        org.LegalFormUuid = (Guid)model.LegalFormUuid;

        try
        {
            await db.SaveChangesAsync();
            return UpdateUserResult.Success(org.Uuid);
        }
        catch (DbUpdateException ex)
        {
            return UpdateUserResult.UnknownError(model.Uuid, ex.ToString());
        }
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        if (await db.Branches.AnyAsync(m => m.OrganizationUuid == uuid))
            return DeleteUserResult.UnknownError(uuid, "Для организации существуют филиалы"); 

        var org = await db.Organizations.FirstOrDefaultAsync(o => o.Uuid == uuid);
        if (org == null) return DeleteUserResult.UserNotFound(uuid);
        
        db.Organizations.Remove(org);
        try
        {
            await db.SaveChangesAsync();
            return DeleteUserResult.Success(uuid);
        }
        catch (Exception ex)
        {
            return DeleteUserResult.UnknownError(uuid, ex.ToString());
        }
    }
}