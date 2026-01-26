using System.Security.Claims;

namespace LMS.Models;

public class AuthModel(ClaimsPrincipal? claimsPrincipal, string? errorMessage)
{
    public ClaimsPrincipal? ClaimsPrincipal { get; } = claimsPrincipal;
    public string? ErrorMessage { get; } = errorMessage;
}