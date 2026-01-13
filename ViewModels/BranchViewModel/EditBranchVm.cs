using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.BranchViewModel;

public class EditBranchVm
{
    public Guid Uuid { get; set; }
    
    [Required(ErrorMessage = "Организация обязательна")]
    public Guid OrganizationUuid { get; set; }
    
    [Required(ErrorMessage = "Название обязательно")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Код обязателен")]
    public string BranchCode { get; set; }
    
    [Required(ErrorMessage = "Статус обязателен")]
    public bool Status { get; set; }
    
    [Required(ErrorMessage = "Регион обязателен")]
    public string? Region { get; set; }  
    
    [Required(ErrorMessage = "По умолчанию или нет обязательно")]
    public bool IsDefault { get; set; }
    
    [Required(ErrorMessage = "Город обязателен")]
    public string? City { get; set; }
    
    [Required(ErrorMessage = "Улица обязательна")]
    public string? Street { get; set; }  
    
    [Required(ErrorMessage = "Номер дома обязательный")]
    public string? HouseNumber { get; set; }
    
    [Required(ErrorMessage = "Часовой пояс обязательный")]
    public int Timezone { get; set; }
}