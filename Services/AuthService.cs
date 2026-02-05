using System.Security.Claims;
using LMS.Data;
using LMS.Data.Entities;
using LMS.Models;
using LMS.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class AuthService(
    IDbContextFactory<DatabaseContext> dbFactory,
    IPasswordHasher<Data.Entities.User.User> passwordHasher)
{
    public async Task<AuthResult> AuthenticateUser(string login, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login);
        
        // Если такого пользователя не существует.
        if (user == null)
        {
            // Данный ход сделан для того, чтобы злоумышленники не смогли
            // вычислить по времени выполнения запроса, какой логин существует, а какой нет.
            passwordHasher.HashPassword(new Data.Entities.User.User(), "dummy_pass");
            
            return AuthResult.Failure(AuthErrorCode.ErrInvalidCredentials);
        }

        // Если пользователь не активный, то ему запрещено входить на платформу.
        if (!user.IsActive)
            return AuthResult.Failure(AuthErrorCode.ErrNoActiveAccount);

        // Если у пользователя неверный пароль, то ему запрещено входить на платформу.
        if (passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
            return AuthResult.Failure(AuthErrorCode.ErrInvalidCredentials);

        // TODO: Позже реализовать получение ролей не из БД, а кешированные заранее значения.
        var administratorRole = db.UsersRole.Where(u => u.Name == "Administrator").Select(u => u.Uuid).FirstOrDefault();
        var moderatorRole = db.UsersRole.Where(u => u.Name == "Moderator").Select(u => u.Uuid).FirstOrDefault();
        var employeeRole = db.UsersRole.Where(u => u.Name == "Employee").Select(u => u.Uuid).FirstOrDefault();
        
        // Если роль пользователя администратор тогда проверки завершены.
        if (user.Role == administratorRole)
        {
            // TODO: Реализовать полный сбор данных пользователя и загрузить в кеш.
            return AuthResult.Success(CreatePrincipal(user.Uuid, user.Role));
        }
        
        // Если роль пользователя модератор тогда необходимо загрузить его филиалы, подписку и проверить их действительность.
        if (user.Role == moderatorRole)
        {
            // TODO: Реализовать полный сбор данных пользователя в том числе подписки их действительность, а также филиалы в которых работает данный соторудник.
            return AuthResult.Success(CreatePrincipal(user.Uuid, user.Role));
        }

        // Если роль пользователя обучающийся тогда необходимо загрузить его филиал где он числится, подписку и проверить их действительность.
        if (user.Role == employeeRole)
        {
            // TODO: Реализовать полный сбор данных пользователя в том числе его филиала и подписку на платформу.
            return AuthResult.Success(CreatePrincipal(user.Uuid, user.Role));
        }
        
        return AuthResult.Failure(AuthErrorCode.ErrInvalidCredentials);
    }
    
    private static ClaimsPrincipal CreatePrincipal(Guid uuid, Guid role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, uuid.ToString()),
            new (ClaimTypes.Role, role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}