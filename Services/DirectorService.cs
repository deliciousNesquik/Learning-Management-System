using LMS.Components.Table;
using LMS.Data;
using LMS.Data.Models;
using LMS.Interfaces;
using LMS.Models.UserManipulationResult;
using LMS.ViewModels.DirectorViewModel;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class DirectorService(IDbContextFactory<DatabaseContext> dbFactory) : IDirectorService
{
    public async Task<PagedResult<DirectorListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        IQueryable<Director> q = db.Directors.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            q = q.Where(d =>
                EF.Functions.Like(d.Surname, $"%{search}%") ||
                EF.Functions.Like(d.Name, $"%{search}%") ||
                EF.Functions.Like(d.Patronymic!, $"%{search}%") ||
                EF.Functions.Like(d.Post, $"%{search}%")
            );

            // предположение:
            // поиск по UUID — отдельной веткой, если search можно распарсить в Guid
        }

        q = (query.SortBy, query.SortDesc) switch
        {
            ("uuid", false) => q.OrderBy(d => d.Uuid),
            ("uuid", true) => q.OrderByDescending(d => d.Uuid),
            ("post", false) => q.OrderBy(d => d.Post),
            ("post", true) => q.OrderByDescending(d => d.Post),
            ("created_at", false) => q.OrderBy(d => d.CreatedAt),
            ("created_at", true) => q.OrderByDescending(d => d.CreatedAt),
            ("full_name", false) => q.OrderBy(d => d.Surname).ThenBy(d => d.Name),
            ("full_name", true) => q.OrderByDescending(d => d.Surname).ThenByDescending(d => d.Name),
            _ => q.OrderByDescending(d => d.CreatedAt)
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DirectorListItemVm
            {
                Uuid = d.Uuid,
                Post = d.Post,
                Surname = d.Surname,
                Name = d.Name,
                Patronymic = d.Patronymic ?? "",

                OrganizationName = string.Join(", ",
                    d.BranchesDirectors
                        .Select(bd => bd.Branch.Organization.Name)
                        .Distinct()),

                OrganizationUuid = d.BranchesDirectors
                    .Select(bd => bd.Branch.Organization.Uuid)
                    .Distinct()
                    .FirstOrDefault(),

                BranchesNames = string.Join(", ",
                    d.BranchesDirectors
                        .Select(bd => bd.Branch.Name)),

                BranchesUuids = d.BranchesDirectors
                    .Select(bd => bd.Branch.Uuid)
                    .ToList()
            })
            .ToListAsync();

        return new PagedResult<DirectorListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<CreateUserResult> CreateAsync(CreateDirectorVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var surname = model.Surname.Trim();
        var name = model.Name.Trim();
        var patronymic = model.Patronymic?.Trim();

        bool alreadyExists = await db.Set<BranchesDirectors>()
            .AnyAsync(bd =>
                model.BranchesUuids.Contains(bd.BranchUuid) &&
                bd.Director.Surname == surname &&
                bd.Director.Name == name &&
                (bd.Director.Patronymic ?? "") == (patronymic ?? "")
            );

        if (alreadyExists)
            return CreateUserResult.LoginAlreadyExists();

        var director = new Director
        {
            Post = model.Post,
            Surname = surname,
            Name = name,
            Patronymic = patronymic,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            db.Directors.Add(director);

            foreach (var branchUuid in model.BranchesUuids.Distinct())
            {
                db.BranchesDirectors.Add(new BranchesDirectors
                {
                    DirectorsUuid = director.Uuid,
                    BranchUuid = branchUuid
                });
            }

            await db.SaveChangesAsync();
            return CreateUserResult.Success(director.Uuid);
        }
        catch (DbUpdateException ex)
        {
            return CreateUserResult.UnknownError(ex.Message);
        }
    }

    public async Task<UpdateUserResult> UpdateAsync(EditDirectorVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
    
        // Загружаем директора
        var director = await db.Directors.FirstOrDefaultAsync(m => m.Uuid == model.Uuid);
        if (director == null)
            return UpdateUserResult.UserNotFound(model.Uuid);

        // 1. Проверка на дубликат (если ФИО изменилось, не занято ли оно в новых филиалах другим человеком)
        string targetSurname = model.Surname.Trim().ToLower();
        string targetName = model.Name.Trim().ToLower();
        string targetPatronymic = model.Patronymic?.Trim().ToLower() ?? "";

        bool alreadyExists = await db.Set<BranchesDirectors>()
            .AnyAsync(bd => 
                model.BranchesUuids.Contains(bd.BranchUuid) && 
                bd.DirectorsUuid != director.Uuid && // Не считаем самого себя
                bd.Director.Surname.ToLower() == targetSurname &&
                bd.Director.Name.ToLower() == targetName &&
                (bd.Director.Patronymic ?? "").ToLower() == targetPatronymic
            );

        if (alreadyExists)
            return UpdateUserResult.UnknownError(model.Uuid, "Директор с таким ФИО уже привязан к одному из выбранных филиалов");

        try
        {
            // 2. Обновляем основные поля
            director.Post = model.Post;
            director.Surname = model.Surname;
            director.Name = model.Name;
            director.Patronymic = model.Patronymic;

            // 3. Обновляем связи с филиалами
            // Сначала удаляем все текущие связи директора
            var currentLinks = db.Set<BranchesDirectors>().Where(bd => bd.DirectorsUuid == director.Uuid);
            db.Set<BranchesDirectors>().RemoveRange(currentLinks);

            // Затем добавляем новые
            foreach (var branchUuid in model.BranchesUuids.Distinct())
            {
                db.Set<BranchesDirectors>().Add(new BranchesDirectors
                {
                    DirectorsUuid = director.Uuid,
                    BranchUuid = branchUuid
                });
            }
            
            await db.SaveChangesAsync();
            return UpdateUserResult.Success(model.Uuid);
        }
        catch (DbUpdateException ex)
        {
            return UpdateUserResult.UnknownError(model.Uuid, ex.Message);
        }
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid userUuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var director = await db.Directors.FirstOrDefaultAsync(m => m.Uuid == userUuid);

        if (director == null)
            return DeleteUserResult.UserNotFound(userUuid);

        try
        {
            // Вручную удаляем связи, если в БД не настроен Cascade Delete
            var links = db.Set<BranchesDirectors>().Where(bd => bd.DirectorsUuid == userUuid);
            db.Set<BranchesDirectors>().RemoveRange(links);

            // Удаляем самого директора
            db.Directors.Remove(director);
        
            await db.SaveChangesAsync();
            return DeleteUserResult.Success(userUuid);
        }
        catch (Exception ex) 
        { 
            return DeleteUserResult.UnknownError(userUuid, ex.Message); 
        }
    }

    #region NotUsed

    public Task<object> GetStatsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResetUserPasswordResult> ResetPasswordAsync(Guid userUuid, string newPassword)
    {
        throw new NotImplementedException();
    }

    #endregion
}