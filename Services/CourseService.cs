using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs.CardsView;
using LMS.DTOs.Course;
using LMS.DTOs.TableView;
using LMS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class CourseService(IDbContextFactory<DatabaseContext> dbFactory) : ICourseService
{
    public async Task<PagedResult<CourseListItemVm>> GetCoursesForCardsAsync(CardQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var q = db.Courses
            .Include(c => c.Category)
            .Include(c => c.Author)
            .Include(c => c.Materials)
            .Include(c => c.Assessments)
            .AsNoTracking();

        // -------------------------
        // 1. ПОИСК
        // -------------------------
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            q = q.Where(c =>
                c.Name.ToLower().Contains(search) ||
                (c.Description != null && c.Description.ToLower().Contains(search))
            );
        }

        // -------------------------
        // 2. ФИЛЬТРЫ
        // -------------------------
        if (query.Filters.TryGetValue("category", out var categoryFilter))
        {
            if (categoryFilter is string singleCategory && !string.IsNullOrEmpty(singleCategory))
                q = q.Where(c => c.Category.Name == singleCategory);
            else if (categoryFilter is IEnumerable<string> categories && categories.Any())
                q = q.Where(c => categories.Contains(c.Category.Name));
        }

        if (query.Filters.TryGetValue("author", out var authorFilter))
        {
            if (authorFilter is string singleAuthor && !string.IsNullOrEmpty(singleAuthor))
                q = q.Where(c => c.Author.Login == singleAuthor);
            else if (authorFilter is IEnumerable<string> authors && authors.Any())
                q = q.Where(c => authors.Contains(c.Author.Login));
        }

        // -------------------------
        // 3. TOTAL COUNT (ВАЖНО!)
        // -------------------------
        var totalCount = await q.CountAsync();

        // -------------------------
        // 4. СОРТИРОВКА
        // -------------------------
        q = query.SortBy?.ToLower() switch
        {
            "name" => query.SortDesc
                ? q.OrderByDescending(c => c.Name)
                : q.OrderBy(c => c.Name),

            "create_date" => query.SortDesc
                ? q.OrderByDescending(c => c.CreatedAt)
                : q.OrderBy(c => c.CreatedAt),

            "category" => query.SortDesc
                ? q.OrderByDescending(c => c.Category.Name)
                : q.OrderBy(c => c.Category.Name),

            _ => q.OrderByDescending(c => c.CreatedAt)
        };

        // -------------------------
        // 5. ПАГИНАЦИЯ + ПРОЕКЦИЯ
        // -------------------------
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CourseListItemVm
            {
                Uuid = c.Uuid,
                Name = c.Name,
                CategoryName = c.Category.Name,
                CategoryColor = c.Category.ColorHex,
                ShortDescription = c.Description == null
                    ? ""
                    : c.Description.Length > 100
                        ? c.Description.Substring(0, 100)
                        : c.Description,
                EstimatedDurationMinutes = c.EstimatedDurationMinutes,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = db.AuditHistories
                    .Where(a =>
                        a.TableName == "COURSES" &&
                        a.RecordUuid == c.Uuid &&
                        a.Action == "UPDATE" &&
                        a.ChangedAt > c.CreatedAt)
                    .OrderByDescending(a => a.ChangedAt)
                    .Select(a => (DateTime?)a.ChangedAt)
                    .FirstOrDefault(),
                MaterialCount = c.Materials.Count,
                AssessmentCount = c.Assessments.Count,
                AuthorName = c.Author.Login
            })
            .ToListAsync();

        return new PagedResult<CourseListItemVm>
        {
            Items = items,
            TotalCount = totalCount
        };
    }


    public async Task<Course?> GetCourseWithDetailsAsync(Guid courseUuid)
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        return await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Author)
            .Include(c => c.Materials)
            .ThenInclude(mc => mc.Material)
            .ThenInclude(m => m.Type)
            .Include(c => c.Assessments)
            .ThenInclude(ac => ac.Assessment)
            .FirstOrDefaultAsync(c => c.Uuid == courseUuid);
    }
}