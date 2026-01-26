using LMS.DTOs.Organization;
using LMS.DTOs.TableView;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces;

public interface IOrganizationService
{
    Task<PagedResult<OrganizationListItemVm>> GetPagedAsync(TableQuery query);
    Task<List<LegalFormLookupVm>> GetLegalFormsAsync();
    Task<List<OrganizationNamesLookupVm>> GetAllOrganizationsNamesAsync();
    Task<OrganizationStatsVm> GetStatsAsync();
    
    Task<List<LegalFormLookupVm>> GetAllForLookupAsync(); 

    Task<CreateUserResult> CreateAsync(CreateOrganizationVm model);
    Task<UpdateUserResult> UpdateAsync(EditOrganizationVm model);
    Task<DeleteUserResult> DeleteAsync(Guid uuid);
}