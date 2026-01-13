namespace LMS.Models.UserManipulationResult;

public sealed class ResetUserPasswordResult
{
    public bool IsSuccess { get; }
    public Guid? UserUuid { get; }
    public string? ErrorCode { get; }

    private ResetUserPasswordResult(bool success, Guid? userUuid, string? errorCode)
    {
        IsSuccess = success;
        UserUuid = userUuid;
        ErrorCode = errorCode;
    }

    public static ResetUserPasswordResult Success(Guid? userUuid)
        => new(true, userUuid, null);

    public static ResetUserPasswordResult UserNotFound(Guid uuid)
        => new(false, uuid, "USER UUID NOT FOUND");
    
    public static ResetUserPasswordResult UserInactive(Guid? userUuid)
        => new(false, userUuid, "USER_INACTIVE");
    
    public static ResetUserPasswordResult UnknownError(Guid uuid, string errorCode)
        => new(false, uuid, errorCode);
}