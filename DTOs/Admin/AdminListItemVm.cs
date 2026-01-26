namespace LMS.DTOs.Admin;

public class AdminListItemVm
{
    public Guid Uuid { get; init; }
    public string Login { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string? Patronymic { get; set; }
    
}