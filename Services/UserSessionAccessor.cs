using System.Security.Claims;
using LMS.Components;
using LMS.Models;

namespace LMS.Services;

public class UserSessionAccessor(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
{
    // Кеш прав в рамках текущего Scoped-сервиса
    private Dictionary<string, HashSet<SqlOperation>>? _permissionsCache = null;
    
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


    public async Task<bool> HasPermission(string source, SqlOperation operation)
    {
        if (_permissionsCache == null)
        {
            var userId = UserUuid;
            if (userId != null)
            {
                var userPermissionsService = serviceProvider.GetRequiredService<UserPermissions>();
                // Получаем сервис только в момент вызова метода, а не при создании аксессора
                _permissionsCache = await userPermissionsService.GetAllPermissions(userId.Value);
            }
        }

        if (_permissionsCache!.TryGetValue(source, out var allowedOperations))
        {
            return allowedOperations.Contains(operation);
        }
        return false;
        
    }
}