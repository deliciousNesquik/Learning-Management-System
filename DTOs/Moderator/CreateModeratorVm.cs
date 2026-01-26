using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Moderator;

public class CreateModeratorVm
{
    [Required(ErrorMessage = "Логин обязателен")]
    public string Login { get; set; } = "";

    [Required(ErrorMessage = "Пароль обязателен")]
    public string PlainPassword { get; set; } = "";
    
    [Required(ErrorMessage = "Необходимо указать фамилию.")] 
    public string Surname { get; set; } = "";
    
    [Required(ErrorMessage = "Необходимо указать имя.")] 
    public string Name { get; set; } = "";
    
    [Required(AllowEmptyStrings = true)]
    public string? Patronymic { get; set; } = "";

    [Required(ErrorMessage = "Необходимо выбрать филиал(ы)")]
    public List<Guid>? BranchesUuids { get; set; } = [];
}