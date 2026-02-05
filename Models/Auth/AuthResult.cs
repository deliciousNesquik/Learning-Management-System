using System.Security.Claims;

namespace LMS.Models.Auth;

public sealed class AuthResult
{
    public ClaimsPrincipal? ClaimsPrincipal { get; }
    public AuthErrorCode? ErrorMessage { get; }

    public AuthResult(ClaimsPrincipal? claimsPrincipal, AuthErrorCode? errorMessage)
    {
        ClaimsPrincipal = claimsPrincipal;
        ErrorMessage = errorMessage;
    }
    
    public static AuthResult Success(ClaimsPrincipal claimsPrincipal)
        => new(claimsPrincipal, null);
    
    public static AuthResult Failure(AuthErrorCode? errorMessage)
        => new(null, errorMessage);
}