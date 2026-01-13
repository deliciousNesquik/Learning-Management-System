namespace LMS.Models.UserManipulationResult;

public class UpdateUserResult
{
    public bool IsSuccess { get; }
    public Guid? UserUuid { get; }
    public string? ErrorCode { get; }

    private UpdateUserResult(bool success, Guid? userUuid, string? errorCode)
    {
        IsSuccess = success;
        UserUuid = userUuid;
        ErrorCode = errorCode;
    }

    public static UpdateUserResult Success(Guid? userUuid)
        => new(true, userUuid, null);

    public static UpdateUserResult UserNotFound(Guid? userUuid)
        => new(false, userUuid, "USER UUID NOT FOUND");
    
    public static UpdateUserResult UnknownError(Guid? userUuid, string errorCode)
        => new(false, userUuid, errorCode);
}