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
            .Include(m => m.Category)
            .Include(m => m.Author)
            .Include(m => m.Materials)
            .Include(m => m.Assessments)
            .ThenInclude(cl => cl.Course)
            .AsNoTracking()
            .AsQueryable();
        
        var total = await q.CountAsync();
        
        var raw = await q
            .OrderByDescending(m => m.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CourseListItemVm()
            {
                Uuid = c.Uuid,
                Name = c.Name,
                CategoryName = c.Category.Name,
                CategoryColor = c.Category.ColorHex,
                ShortDescription = c.Description == null ? "" : c.Description.Length > 100 ? c.Description.Substring(0, 100) : c.Description,
                EstimatedDurationMinutes = c.EstimatedDurationMinutes,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = db.AuditHistories
                    .Where(a => a.TableName == "COURSES" 
                                && a.RecordUuid == c.Uuid 
                                && a.Action == "UPDATE" 
                                && a.ChangedAt > c.CreatedAt)
                    .OrderByDescending(a => a.ChangedAt)
                    .Select(a => (DateTime?)a.ChangedAt)
                    .FirstOrDefault(),
                MaterialCount = c.Materials.Count,
                AssessmentCount = c.Assessments.Count,
                AuthorName = $"{c.Author.Login}"
            })
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        
        return new PagedResult<CourseListItemVm>
        {
            Items = raw,
            TotalCount = total,
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