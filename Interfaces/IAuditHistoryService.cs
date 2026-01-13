using LMS.Components.Table;
using LMS.ViewModels.AuditHistoryViewModel;

namespace LMS.Interfaces;

public interface IAuditHistoryService
{
    Task<PagedResult<AuditHistoryListItemVm>> GetPagedAsync(TableQuery query);
}