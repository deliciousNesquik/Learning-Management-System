using LMS.Components.Table;
using LMS.Data;
using LMS.Interfaces;
using LMS.ViewModels.AuditHistoryViewModel;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class AuditHistoryService(
    IDbContextFactory<DatabaseContext> dbFactory)
    : IAuditHistoryService
{
    public async Task<PagedResult<AuditHistoryListItemVm>> GetPagedAsync(TableQuery query)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        var q = db.AuditHistories.AsNoTracking();

        // --- ПОИСК ---
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            q = q.Where(a => 
                a.Uuid.ToString().Contains(query.Search) ||
                a.TableName.Contains(query.Search) ||
                a.RecordUuid.ToString().Contains(query.Search));
        }

        // --- СОРТИРОВКА ---
        q = (query.SortBy, query.SortDesc) switch
        {
            ("table_name", false) => q.OrderBy(a => a.TableName),
            ("table_name", true) => q.OrderByDescending(a => a.TableName),
            
            ("record_uuid", false) => q.OrderBy(a => a.RecordUuid),
            ("record_uuid", true) => q.OrderByDescending(a => a.RecordUuid),
            
            ("action", false) => q.OrderBy(a => a.Action),
            ("action", true) => q.OrderByDescending(a => a.Action),
            
            ("changed_at", false) => q.OrderBy(a => a.ChangedAt),
            ("changed_at", true) => q.OrderByDescending(a => a.ChangedAt),
            
            ("changed_by", false) => q.OrderBy(a => a.ChangedBy),
            ("changed_by", true) => q.OrderByDescending(a => a.ChangedBy),
            
            _ => q.OrderByDescending(a => a.ChangedAt)
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new AuditHistoryListItemVm()
            {
                Uuid = a.Uuid,
                TableName = a.TableName,
                RecordUuid = a.RecordUuid,
                Action = a.Action,
                OldData = (a.OldData == null) ? "Нет данных" : a.OldData,
                NewData = (a.NewData == null) ? "Нет данных" : a.NewData,
                ChangedAt = a.ChangedAt,
                ChangedBy = a.ChangedBy
            })
            .ToListAsync();

        return new PagedResult<AuditHistoryListItemVm>
        {
            Items = items,
            TotalCount = total
        };
    }
}