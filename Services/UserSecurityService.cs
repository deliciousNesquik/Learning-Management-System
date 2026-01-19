using System.Text.Json;
using LMS.Data;
using LMS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class UserSecurityService(DatabaseContext context) : IUserSecurityService
{
    private readonly DatabaseContext _context = context;

    public async Task<DateTime?> GetLastPasswordChangeDateAsync(Guid userUuid)
    {
        try
        {
            // Получаем все релевантные записи аудита для этого пользователя
            var allUpdates = await _context.AuditHistories
                .Where(ah => ah.RecordUuid == userUuid &&
                             ah.Action == "UPDATE")
                .OrderByDescending(ah => ah.ChangedAt)
                .Select(ah => new
                {
                    ah.ChangedAt,
                    ah.OldData,
                    ah.NewData
                })
                .ToListAsync();

            // Фильтруем на клиенте - ищем где изменился пароль
            foreach (var update in allUpdates)
                try
                {
                    if (update.OldData != null && update.NewData != null)
                    {
                        // Извлекаем значения паролей из JSONB
                        var oldPassword = update.OldData.RootElement.TryGetProperty("password", out JsonElement oldPass)
                            ? oldPass.GetString()
                            : null;

                        var newPassword = update.NewData.RootElement.TryGetProperty("password", out JsonElement newPass)
                            ? newPass.GetString()
                            : null;

                        // Проверяем, действительно ли пароль изменился
                        if (!string.IsNullOrEmpty(oldPassword) &&
                            !string.IsNullOrEmpty(newPassword) &&
                            oldPassword != newPassword)
                            return update.ChangedAt;
                    }
                }
                catch (Exception)
                {
                    // Пропускаем записи с некорректными JSON данными
                }

            // Если не нашли изменений пароля, возвращаем дату создания аккаунта
            return await _context.AuditHistories
                .Where(ah => ah.RecordUuid == userUuid &&
                             ah.Action == "INSERT")
                .OrderBy(ah => ah.ChangedAt)
                .Select(ah => (DateTime?)ah.ChangedAt)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в GetLastPasswordChangeDateAsync: {ex.Message}");
            throw;
        }
    }
}