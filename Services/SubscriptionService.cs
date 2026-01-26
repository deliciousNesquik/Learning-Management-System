using LMS.Components.Table;
using LMS.Data;
using LMS.Data.Models;
using LMS.DTOs.Subscription;
using LMS.Interfaces;
using LMS.Models.UserManipulationResult;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class SubscriptionService(IDbContextFactory<DatabaseContext> dbFactory) : ISubscriptionService
{
    public async Task<PagedResult<SubscriptionListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var q = db.Subscriptions
            .AsNoTracking()
            .Include(s => s.Branch)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            q = q.Where(s => 
                s.Uuid.ToString().Contains(query.Search) ||
                s.Name.Contains(query.Search) ||
                s.BranchUuid.ToString().Contains(query.Search) 
                //|| s.Branch.Name.ToString().Contains(query.Search)
                );
        }

        // Сортировка
        q = (query.SortBy?.ToLower(), query.SortDesc) switch
        {
            ("start_date", false) => q.OrderBy(s => s.StartDate),
            ("start_date", true) => q.OrderByDescending(s => s.StartDate),
            
            ("end_date", false) => q.OrderBy(s => s.EndDate),
            ("end_date", true) => q.OrderByDescending(s => s.EndDate),
            
            ("limit", false) => q.OrderBy(s => s.EmployeesLimit),
            ("limit", true) => q.OrderByDescending(s => s.EmployeesLimit),
            
            ("status", false) => q.OrderBy(s => s.IsActive),
            ("status", true) => q.OrderByDescending(s => s.IsActive),
            
            ("price", false) => q.OrderBy(s => s.Price),
            ("price", true) => q.OrderByDescending(s => s.Price),
            
            ("count_courses", false) => q.OrderBy(s => s.Courses.Count(c => c.SubscriptionUuid == s.Uuid)),
            ("count_courses", true) => q.OrderByDescending(s => s.Courses.Count(c => c.SubscriptionUuid == s.Uuid)),
            
            ("created_at", false) => q.OrderBy(s => s.CreatedAt),
            ("created_at", true) => q.OrderByDescending(s => s.CreatedAt),
            _ => q.OrderByDescending(s => s.CreatedAt)
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new SubscriptionListItemVm
            {
                Uuid = s.Uuid,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                EmployeesLimit = s.EmployeesLimit,
                IsActive = s.IsActive,
                Price = s.Price,
                Currency = s.Currency,
                BranchName = s.Branch.Name,
                BranchUuid = s.Branch.Uuid,
                CoursesCount = db.Set<SubscriptionCourseList>().Count(c => c.SubscriptionUuid == s.Uuid),
                Courses = db.Set<SubscriptionCourseList>().Where(sc => sc.SubscriptionUuid == s.Uuid).Select(a => a.CourseUuid).ToList(),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<SubscriptionListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<CreateUserResult> CreateAsync(CreateSubscriptionVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        var subscription = new Subscription
        {
            Uuid = Guid.NewGuid(),
            Name = model.Name.Trim(),
            BranchUuid = model.BranchUuid,
            StartDate = model.StartDate.ToUniversalTime(),
            EndDate = model.EndDate.ToUniversalTime(),
            EmployeesLimit = model.EmployeesLimit,
            Price = model.Price,
            Currency = model.Currency,
            BillingPeriod = model.BillingPeriod,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        // Добавляем выбранные курсы
        if (model.SelectedCourseUuids != null && model.SelectedCourseUuids.Any())
        {
            foreach (var courseUuid in model.SelectedCourseUuids)
            {
                subscription.Courses.Add(new SubscriptionCourseList
                {
                    Uuid = Guid.NewGuid(),
                    CourseUuid = courseUuid,
                    SubscriptionUuid = subscription.Uuid
                });
            }
        }

        try
        {
            db.Subscriptions.Add(subscription);
            await db.SaveChangesAsync();
            return CreateUserResult.Success(subscription.Uuid);
        }
        catch (Exception ex)
        {
            return CreateUserResult.UnknownError(ex.Message);
        }
    }

    public async Task<UpdateUserResult> UpdateAsync(EditSubscriptionVm model)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        // Загружаем подписку вместе со списком курсов для синхронизации
        var subscription = await db.Subscriptions
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.Uuid == model.Uuid);

        if (subscription == null) return UpdateUserResult.UserNotFound(model.Uuid);

        try
        {
            subscription.Name = model.Name.Trim();
            subscription.StartDate = model.StartDate.ToUniversalTime();
            subscription.EndDate = model.EndDate.ToUniversalTime();
            subscription.EmployeesLimit = model.EmployeesLimit;
            subscription.Price = model.Price;
            subscription.Currency = model.Currency;
            subscription.BillingPeriod = model.BillingPeriod;
            subscription.IsActive = model.IsActive;
            subscription.BranchUuid = model.BranchUuid;

            // --- СИНХРОНИЗАЦИЯ КУРСОВ ---
            // 1. Удаляем те, которых больше нет в списке
            var coursesToRemove = subscription.Courses
                .Where(c => !model.SelectedCourseUuids.Contains(c.CourseUuid))
                .ToList();
            foreach (var c in coursesToRemove) db.Set<SubscriptionCourseList>().Remove(c);

            // 2. Добавляем новые, которых еще нет в базе
            var currentCourseIds = subscription.Courses.Select(c => c.CourseUuid).ToList();
            var coursesToAdd = model.SelectedCourseUuids
                .Where(id => !currentCourseIds.Contains(id))
                .Select(id => new SubscriptionCourseList
                {
                    Uuid = Guid.NewGuid(),
                    CourseUuid = id,
                    SubscriptionUuid = subscription.Uuid
                });
            
            await db.Set<SubscriptionCourseList>().AddRangeAsync(coursesToAdd);

            await db.SaveChangesAsync();
            return UpdateUserResult.Success(model.Uuid);
        }
        catch (Exception ex)
        {
            return UpdateUserResult.UnknownError(model.Uuid, ex.Message);
        }
    }

    public async Task<DeleteUserResult> DeleteAsync(Guid uuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var subscription = await db.Subscriptions.FirstOrDefaultAsync(s => s.Uuid == uuid);
        
        if (subscription == null) return DeleteUserResult.UserNotFound(uuid);

        try
        {
            // Сначала удаляем связи с курсами (если не настроено каскадное удаление в БД)
            var links = db.Set<SubscriptionCourseList>().Where(l => l.SubscriptionUuid == uuid);
            db.Set<SubscriptionCourseList>().RemoveRange(links);

            db.Subscriptions.Remove(subscription);
            await db.SaveChangesAsync();
            return DeleteUserResult.Success(uuid);
        }
        catch (Exception ex)
        {
            return DeleteUserResult.UnknownError(uuid, ex.Message);
        }
    }

    public async Task<List<CourseLookupVm>> GetAllCoursesForLookupAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        // Предполагается, что таблица Courses существует
        return await db.Set<Course>() 
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CourseLookupVm(c.Uuid, c.Name))
            .ToListAsync();
    }

    public async Task<List<Guid>> GetAllAvailableCourses(Guid branchUuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
    
        // Получаем текущую дату для проверки срока действия
        var now = DateTime.UtcNow;

        return await db.Subscriptions
            .AsNoTracking()
            // Фильтруем подписки: для нужного филиала, активные и не просроченные
            .Where(s => s.BranchUuid == branchUuid && s.IsActive && s.EndDate > now)
            // Выбираем курсы из связующей таблицы SubscriptionCourseList
            .SelectMany(s => s.Courses) 
            // Нам нужны только UUID самих курсов
            .Select(sc => sc.CourseUuid)
            // Убираем дубликаты (если один курс есть в нескольких активных подписках)
            .Distinct()
            .ToListAsync();
    }
}