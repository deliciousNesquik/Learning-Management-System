namespace LMS.Models.UserManipulationResult;

public sealed class ResetUserPasswordResult
{
    public bool IsSuccess { get; }
    public Guid? UserUuid { get; }
    public string? ErrorCode { get; }

    private ResetUserPasswordResult(bool success, Guid? uuid, string? errorCode)
    {
        IsSuccess = success;
        UserUuid = uuid;
        ErrorCode = errorCode;
    }

    public static ResetUserPasswordResult Success(Guid? uuid)
        => new(true, uuid, null);

    public static ResetUserPasswordResult UserNotFound(Guid uuid)
        => new(false, uuid, $"Пользователь с {uuid} не существует");
    
    public static ResetUserPasswordResult UserInactive(Guid? uuid)
        => new(false, uuid, $"Пользователь с {uuid} не активный");
    
    public static ResetUserPasswordResult UnknownError(Guid uuid, string errorCode)
        => new(false, uuid, errorCode);
}