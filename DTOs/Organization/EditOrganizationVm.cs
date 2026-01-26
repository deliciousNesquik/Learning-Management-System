using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Organization;

public class EditOrganizationVm : CreateOrganizationVm
{
    public Guid Uuid { get; set; }
    
    [Required(ErrorMessage = "Название организации обязательно")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Почта организации обязательна")]
    public string Mail { get; set; }
    
    [Required(ErrorMessage = "Контакты организации обязательны")]
    public string Contacts { get; set; }
    
    [Required(ErrorMessage = "ИНН организации обязательно")]
    public long TaxPayer { get; set; }
    
    [Required(ErrorMessage = "Форма организации обязательна")]
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