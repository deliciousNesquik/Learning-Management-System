using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs.Course;
using LMS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class CourseService(IDbContextFactory<DatabaseContext> dbFactory) : ICourseService
{
    public async Task<List<CourseListItemVm>> GetCoursesForCardsAsync()
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Author)
            .Include(c => c.Materials)
            .Include(c => c.Assessments)
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
                UpdatedAt = context.AuditHistories
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