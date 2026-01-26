using LMS.Components.Table;
using LMS.DTOs.Branch;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces;

public interface IBranchService
{
    Task<List<BranchNamesLookupVm>> GetAllBranchesAsync();
    Task<PagedResult<BranchListItemVm>> GetPagedAsync(TableQuery query);
    Task<CreateUserResult> CreateAsync(CreateBranchVm model);
    Task<UpdateUserResult> UpdateAsync(EditBranchVm model);
    Task<DeleteUserResult> DeleteAsync(Guid uuid);
}