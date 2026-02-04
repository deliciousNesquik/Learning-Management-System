namespace LMS.DTOs.User;

public class UserDto
{
    public Guid Uuid { get; init; }
    
    public Guid Role { get; set; }
    public string Login { get; set; } = "";
    public string PlainPassword { get; set; } = "";
    
    public required string Surname { get; set; }
    public required string Name { get; set; }
    public string? GivenName { get; set; }
    
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
    
    public bool IsActive { get; set; }
    
    
    /// <summary>
    /// Поле типа jsonb содержащее дополнительную информацию о пользователе
    /// в формате (json). Пример использования данного поля:
    /// {
    ///     post: "Пожарник",
    ///     affiliation: "ООО 'Олимп'",
    ///     insurance : "5346573252",
    /// }
    /// </summary>
    public JsonContent? AdditionalFields { get; set; }
}