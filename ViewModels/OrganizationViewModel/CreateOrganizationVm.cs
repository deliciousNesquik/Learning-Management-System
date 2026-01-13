using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.OrganizationViewModel;

public class CreateOrganizationVm
{
    [Required(ErrorMessage = "Название обязательно")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Почта организации обязательна")]
    public string Mail { get; set; }
    
    [Required(ErrorMessage = "Контакты организации обязательны")]
    public string Contacts { get; set; }
    
    [Required(ErrorMessage = "ИНН обязателен")]
    public long TaxPayer { get; set; }
    
    [Required(ErrorMessage = "Выберите форму организации")]
    public Guid? LegalFormUuid { get; set; }
    
    public string? LicenseNumber { get; set; }

    public DateTime? LicenseValidFrom { get; set; }
    public DateTime? LicenseValidTo { get; set; }
    
    public string? AccreditationInfo { get; set; }
    public int TimeZone { get; set; }

    [Required(ErrorMessage = "Регион обязателен")]
    public string? Region { get; set; }  
    
    [Required(ErrorMessage = "Город обязателен")]
    public string? City { get; set; }
    
    [Required(ErrorMessage = "Улица обязательна")]
    public string? Street { get; set; }  
    
    [Required(ErrorMessage = "Номер дома обязательный")]
    public string? HouseNumber { get; set; }

    
}