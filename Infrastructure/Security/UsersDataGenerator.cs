using System.Security.Cryptography;

namespace LMS.Infrastructure.Security;

/// <summary>
/// Обеспечивает логику генерации учетных данных для пользователей.
/// </summary>
public static class UsersDataGenerator
{
    private const string Lowercase = "abcdefghijkmnopqrstuvwxyz"; // исключены неоднозначные l
    private const string Uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";    // исключены неоднозначные O, I
    private const string Digits = "23456789";                   // исключены 0, 1
    private const string Special = "!@#$%^*";
    
    private static readonly char[] AllChars = $"{Lowercase}{Uppercase}{Digits}{Special}".ToCharArray();

    /// <summary>
    /// Генерирует уникальный логин на основе даты и случайного суффикса.
    /// Формат: [prefix]ddMMyy_XXXX
    /// </summary>
    /// <param name="prefix">Опциональный префикс (например, 'moder_')</param>
    /// <returns>Строка логина</returns>
    public static string GenerateLogin(string? prefix = null)
    {
        // Используем Random.Shared для потокобезопасности в веб-приложениях
        var suffix = Random.Shared.Next(0, 10000);
        
        return $"{prefix}{DateTime.Now:ddMMyy}_{suffix:D4}";
    }

    /// <summary>
    /// Генерирует криптографически стойкий пароль.
    /// Гарантирует наличие символов разных регистров и цифр.
    /// </summary>
    /// <param name="length">Длина пароля (не менее 8 символов)</param>
    public static string GeneratePassword(int length = 12)
    {
        if (length < 8) length = 8;

        // string.Create позволяет записывать символы напрямую в память будущей строки
        return string.Create(length, AllChars, (buffer, chars) =>
        {
            // 1. Заполняем буфер случайными безопасными значениями
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }
            
            // Вставляем их в случайные позиции
            buffer[RandomNumberGenerator.GetInt32(buffer.Length)] = Lowercase[RandomNumberGenerator.GetInt32(Lowercase.Length)];
            buffer[RandomNumberGenerator.GetInt32(buffer.Length)] = Uppercase[RandomNumberGenerator.GetInt32(Uppercase.Length)];
            buffer[RandomNumberGenerator.GetInt32(buffer.Length)] = Digits[RandomNumberGenerator.GetInt32(Digits.Length)];
            
            // Опционально использовать данную строку.
            //buffer[RandomNumberGenerator.GetInt32(buffer.Length)] = Special[RandomNumberGenerator.GetInt32(Special.Length)];
        });
    }
}