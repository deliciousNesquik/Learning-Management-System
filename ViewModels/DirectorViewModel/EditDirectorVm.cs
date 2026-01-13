using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.DirectorViewModel;

public class EditDirectorVm
{
    public Guid Uuid { get; set; }
    
    [Required(ErrorMessage = "Необходимо выбрать филиал(ы)")]
    public List<Guid> BranchesUuids { get; set; }
    
    [Required(ErrorMessage = "Должность обязательна")]
    public string Post { get; set; } = "";
    
    [Required(ErrorMessage = "Фамилия обязательна")]
    public string Surname { get; set; } = "";
    
    [Required(ErrorMessage = "Имя обязательно")]
    public string Name { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";
}