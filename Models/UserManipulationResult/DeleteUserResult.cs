namespace LMS.Models.UserManipulationResult;

public class DeleteUserResult
{
    public bool IsSuccess { get; }
    public Guid? UserUuid { get; }
    public string? ErrorCode { get; }

    private DeleteUserResult(bool isSuccess, Guid? userUuid, string? errorCode)
    {
        IsSuccess = isSuccess;
        UserUuid = userUuid;
        ErrorCode = errorCode;
    }

    public static DeleteUserResult Success(Guid uuid)
        => new(true, uuid, null);

    public static DeleteUserResult UserNotFound(Guid uuid)
        => new(false, uuid, "USER UUID NOT FOUND");
    
    public static DeleteUserResult UnknownError(Guid uuid, string errorCode)
        => new(false, uuid, errorCode);
}