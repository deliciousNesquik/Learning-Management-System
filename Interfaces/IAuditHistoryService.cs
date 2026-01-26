using LMS.Components.Table;
using LMS.DTOs.AuditHistory;

namespace LMS.Interfaces;

public interface IAuditHistoryService
{
    Task<PagedResult<AuditHistoryListItemVm>> GetPagedAsync(TableQuery query);
}