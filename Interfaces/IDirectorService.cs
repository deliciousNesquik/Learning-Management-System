using LMS.ViewModels.DirectorViewModel;

namespace LMS.Interfaces;

public interface IDirectorService : IUserService<DirectorListItemVm, object, CreateDirectorVm, EditDirectorVm>
{
    
}