using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Moderator;

public class EditModeratorVm
{
    public Guid Uuid { get; init; }

    [Required(ErrorMessage = "Необходимо указать логин.")]
    public string Login { get; set; } = "";

    [Required(AllowEmptyStrings = true)] 
    public bool IsActive { get; set; } = true;
    
    [Required(ErrorMessage = "Необходимо указать фамилию.")] 
    public string Surname { get; set; } = "";
    
    [Required(ErrorMessage = "Необходимо указать имя.")] 
    public string Name { get; set; } = "";
    
    [Required(AllowEmptyStrings = true)]
    public string? Patronymic { get; set; } = "";

    [Required(ErrorMessage = "Необходимо указать филиал(ы)")]
    public List<Guid>? BranchesUuids { get; set; } = [];
}