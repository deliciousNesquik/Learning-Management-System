using System.Security.Claims;
using LMS.Data;
using LMS.Data.Entities;
using LMS.Models;
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
    public async Task<AuthModel> AuthenticateUser(string login, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // 1. Администратор
        var administrator = await db.Administrators.FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        if (administrator != null &&
            adminHasher.VerifyHashedPassword(administrator, administrator.Password.Trim(), password.Trim()) !=
            PasswordVerificationResult.Failed)
            return new AuthModel(
                CreatePrincipal(administrator.Uuid, administrator.Login, "Admin", administrator.Surname,
                    administrator.Name, administrator.Patronymic), "");

        // 2. Модератор
        var moderator = await db.Moderators.FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        if (moderator != null &&
            modHasher.VerifyHashedPassword(moderator, moderator.Password.Trim(), password.Trim()) !=
            PasswordVerificationResult.Failed)
        {
            var moderatorBranchUuids = await db.BranchesModerators
                .Where(bm => bm.ModeratorUuid == moderator.Uuid)
                .Select(bm => bm.BranchUuid)
                .ToListAsync();

            if (moderatorBranchUuids.Count == 0)
                return new AuthModel(null, "ERR_NO_ORGANIZATION"); // Код: Нет организации

            var activeBranchUuids = await db.Subscriptions
                .Where(s => moderatorBranchUuids.Contains(s.BranchUuid) && s.IsActive && s.EndDate > DateTime.Now)
                .Select(s => s.BranchUuid)
                .Distinct()
                .ToListAsync();

            if (activeBranchUuids.Any())
                return new AuthModel(
                    CreatePrincipal(moderator.Uuid, moderator.Login, "Moderator", moderator.Surname, moderator.Name,
                        moderator.Patronymic, activeBranchUuids), "");

            return new AuthModel(null, "ERR_SUBSCRIPTION_EXPIRED"); // Код: Подписка истекла
        }

        // 3. Обучающийся
        var employee = await db.Employees.FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        if (employee != null && empHasher.VerifyHashedPassword(employee, employee.Password.Trim(), password.Trim()) !=
            PasswordVerificationResult.Failed)
            return new AuthModel(
                CreatePrincipal(employee.Uuid, employee.Login, "Employee", employee.Surname, employee.Name,
                    employee.Patronymic), "");

        return new AuthModel(null, "ERR_INVALID_CREDENTIALS"); // Код: Неверный логин/пароль
    }

    private static ClaimsPrincipal CreatePrincipal(
        Guid uuid,
        string login,
        string role,
        string surname, string name, string? patronymic,
        List<Guid>? activeBranchUuids = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.SerialNumber, uuid.ToString()), // UUID пользователя.
            new(ClaimTypes.NameIdentifier, login), // login пользователя.
            new(ClaimTypes.Role, role), // role пользователя.
            new(ClaimTypes.Surname, surname), // surname пользователя.
            new(ClaimTypes.Name, name), // name пользователя.
            new(ClaimTypes.GivenName, patronymic ?? "") // patronymic/given_name пользователя.
        };
        
        if (activeBranchUuids != null && activeBranchUuids.Count != 0)
        {
            // Добавляем список всех доступных филиалов
            claims.AddRange(activeBranchUuids.Select(id => new Claim("BranchUuid", id.ToString())));

            // УСТАНАВЛИВАЕМ ПЕРВЫЙ ПО УМОЛЧАНИЮ
            // Мы берем первый ID из списка и записываем его в специальный клейм
            claims.Add(new Claim("ActiveBranchUuid", activeBranchUuids.First().ToString()));
        }
        

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}