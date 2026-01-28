using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs.CardsView;
using LMS.DTOs.Material;
using LMS.DTOs.TableView;
using LMS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class MaterialService(IDbContextFactory<DatabaseContext> dbFactory, IFileStorageService storageService)
{

    public async Task<PagedResult<MaterialListItemDto>> GetMaterialsAsync(CardQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var q = db.Materials
            .Include(m => m.Type)
            .Include(m => m.CourseLinks)
                .ThenInclude(cl => cl.Course)
            .AsNoTracking()
            .AsQueryable();
        

        var total = await q.CountAsync();
        var rawData = await q
            .OrderByDescending(m => m.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(m => new 
            {
                m.Uuid,
                m.S3Key,
                TypeName = m.Type.Name,
                m.SizeBytes,
                m.CreatedAt,
                Courses = m.CourseLinks.Select(cl => cl.Course.Name).ToList(),
                FirstCourseId = m.CourseLinks.Select(cl => cl.CourseUuid).FirstOrDefault()
            })
            .ToListAsync();

        return new PagedResult<MaterialListItemDto>
        {
            Items = rawData.Select(d => new MaterialListItemDto
            {
                Uuid = d.Uuid,
                Title = d.S3Key.Contains('_') ? d.S3Key.Split('_').Last() : d.S3Key,
                TypeName = d.TypeName ?? "Unknown",
                FileSizeBytes = d.SizeBytes ?? 0,
                CreatedAt = d.CreatedAt,
                CourseId = d.FirstCourseId,
                CourseName = string.Join(", ", d.Courses),
                
                S3Key = d.S3Key,
                
                AccessUrl = storageService.GetPresignedUrl(d.S3Key)
            }).ToList(),
            TotalCount = total
        };
    }
}