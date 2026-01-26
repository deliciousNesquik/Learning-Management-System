using LMS.DTOs.Director;

namespace LMS.Interfaces;

public interface IDirectorService : IUserService<DirectorListItemVm, object, CreateDirectorVm, EditDirectorVm>
{
    
}