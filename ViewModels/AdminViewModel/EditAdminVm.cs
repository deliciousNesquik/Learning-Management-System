using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.AdminViewModel;

public class EditAdminVm
{
    public Guid Uuid { get; set; }
    public bool IsActive { get; set; }
    
    [Required]
    public string Login { get; set; } = "";

    [Required] 
    public string Surname { get; set; } = "";
    
    [Required] 
    public string Name { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";
}