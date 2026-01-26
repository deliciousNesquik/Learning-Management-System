using LMS.DTOs.AuditHistory;
using LMS.DTOs.TableView;

namespace LMS.Interfaces;

public interface IAuditHistoryService
{
    Task<PagedResult<AuditHistoryListItemVm>> GetPagedAsync(TableQuery query);
}