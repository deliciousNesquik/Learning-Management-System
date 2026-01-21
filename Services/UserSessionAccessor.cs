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

    public List<Guid>? AllBranchUuids
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindAll("BranchUuid")
                .Select(c => Guid.TryParse(c.Value, out var guid) ? guid : Guid.Empty).Where(id => id != Guid.Empty)
                .ToList();
            return claim;
        }
    }

    public Guid? ActiveBranchUuid
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return null;

            // 1. Проверяем, нет ли в браузере специальной куки "выбранный филиал"
            var cookieValue = context.Request.Cookies["ActiveBranchOverride"];
            if (Guid.TryParse(cookieValue, out var cookieGuid))
            {
                // ОБЯЗАТЕЛЬНО: проверяем, что этот филиал действительно принадлежит пользователю
                if (AllBranchUuids?.Contains(cookieGuid) == true)
                    return cookieGuid;
            }

            // 2. Если куки нет, берем тот, который мы записали в клеймы при логине
            var claim = context.User.FindFirst("ActiveBranchUuid");
            if (claim != null && Guid.TryParse(claim.Value, out var guid))
            {
                return guid;
            }
            return null;
        }
        set
        {
            var context = httpContextAccessor.HttpContext;
            if (context != null && value.HasValue)
            {
                // Записываем выбор пользователя в простую куку
                context.Response.Cookies.Append("ActiveBranchOverride", value.Value.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true, // чтобы нельзя было украсть через JS
                    Secure = true
                });
            }
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