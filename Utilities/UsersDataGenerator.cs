

namespace LMS.Utilities;

public static class UsersDataGenerator
{
    private static readonly Random Postfix = new();
    
    public static string GeneratePassword() => Guid.NewGuid().ToString()[..8];
    public static string GenerateLogin(string? prefix) => $"{prefix ?? ""}{DateTime.Now.Day.ToString()}_{DateTime.Now.Month.ToString()}{DateTime.Now.Year.ToString()[2..4]}_{Postfix.NextInt64(9999)}";
}