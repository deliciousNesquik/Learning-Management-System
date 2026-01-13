using System.Security.Claims;

namespace LMS.Services;

public class UserSessionAccessor(IHttpContextAccessor httpContextAccessor)
{
    public Guid? UserUuid
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.SerialNumber);
            if (claim != null && Guid.TryParse(claim.Value, out var guid))
            {
                return guid;
            }
            return null;
        }
    }

    public string? UserLogin
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }
    }
    
    public string? UserRole
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.Role);
            return claim?.Value;
        }
    }
    
    public string? UserSurname
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.Surname);
            return claim?.Value;
        }
    }
    
    public string? UserName
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.Name);
            return claim?.Value;
        }
    }
    
    public string? UserPatronymic
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.GivenName);
            return claim?.Value;
        }
    }
}