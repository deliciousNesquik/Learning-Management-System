namespace LMS.Models.UserManipulationResult;

public sealed class CreateUserResult
{
    public bool IsSuccess { get; }
    public Guid? UserUuid { get; }
    public string? ErrorCode { get; }

    private CreateUserResult(bool isSuccess, Guid? uuid, string? errorCode)
    {
        IsSuccess = isSuccess;
        UserUuid = uuid;
        ErrorCode = errorCode;
    }

    public static CreateUserResult Success(Guid uuid)
        => new(true, uuid, null);

    public static CreateUserResult LoginAlreadyExists()
        => new(false, null, "Пользователь уже существует");
    
    public static CreateUserResult UnknownError(string errorCode)
        => new(false, null, errorCode);
}