namespace LMS.Models.UserManipulationResult;

public class UpdateUserResult
{
    public bool IsSuccess { get; }
    public Guid? UserUuid { get; }
    public string? ErrorCode { get; }

    private UpdateUserResult(bool success, Guid? uuid, string? errorCode)
    {
        IsSuccess = success;
        UserUuid = uuid;
        ErrorCode = errorCode;
    }

    public static UpdateUserResult Success(Guid? uuid)
        => new(true, uuid, null);

    public static UpdateUserResult UserNotFound(Guid? uuid)
        => new(false, uuid, $"Пользователь с {uuid} не существует");
    
    public static UpdateUserResult UnknownError(Guid? uuid, string errorCode)
        => new(false, uuid, errorCode);
}