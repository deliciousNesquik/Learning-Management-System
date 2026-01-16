using System.Security.Claims;
using LMS.Data;
using LMS.Data.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class AuthService(
    IDbContextFactory<DatabaseContext> dbFactory,
    IPasswordHasher<Administrator> adminHasher,
    IPasswordHasher<Moderator> modHasher,
    IPasswordHasher<Employee> empHasher)
{

    public async Task<ClaimsPrincipal?> AuthenticateUser(string login, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        
        var administrator = await db.Administrators.FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        if (administrator != null && adminHasher.VerifyHashedPassword(administrator, administrator.Password.Trim(), password.Trim()) != PasswordVerificationResult.Failed)
            return CreatePrincipal(administrator.Uuid, administrator.Login, "Admin", administrator.Surname, administrator.Name, administrator.Patronymic);
        
        var moderator = await db.Moderators.FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        if (moderator != null && modHasher.VerifyHashedPassword(moderator, moderator.Password.Trim(), password.Trim()) != PasswordVerificationResult.Failed)
            return CreatePrincipal(moderator.Uuid, moderator.Login, "Moderator", moderator.Surname, moderator.Name, moderator.Patronymic);
        
        var employee = await db.Employees.FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        if (employee != null && empHasher.VerifyHashedPassword(employee, employee.Password.Trim(), password.Trim()) != PasswordVerificationResult.Failed)
            return CreatePrincipal(employee.Uuid, employee.Login, "Employee", employee.Surname, employee.Name, employee.Patronymic);
        
        
        return null;
    }

    private static ClaimsPrincipal CreatePrincipal(Guid uuid, string login, string role, string surname, string name, string? patronymic)
    {
        var claims = new List<Claim>
        {
            
            new(ClaimTypes.SerialNumber, uuid.ToString()), // UUID пользователя.
            new(ClaimTypes.NameIdentifier, login),         // login пользователя.
            new(ClaimTypes.Role, role),                    // role пользователя.
            new(ClaimTypes.Surname, surname),              // surname пользователя.
            new(ClaimTypes.Name, name),                    // name пользователя.
            new(ClaimTypes.GivenName, patronymic ?? "")         // patronymic/given_name пользователя.
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}