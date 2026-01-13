using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.AdminViewModel;

public class CreateAdminVm
{
    [Required(ErrorMessage = "Необходимо указать логин.")]
    public string Login { get; set; } = "";

    [Required(ErrorMessage = "Необходимо указать пароль.")]
    public string PlainPassword { get; set; } = "";

    [Required(ErrorMessage = "Необходимо указать фамилию.")] 
    public string Surname { get; set; } = "";
    
    [Required(ErrorMessage = "Необходимо указать имя.")] 
    public string Name { get; set; } = "";
    
    [Required(AllowEmptyStrings = true)]
    public string? Patronymic { get; set; } = "";
}