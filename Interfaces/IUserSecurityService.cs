namespace LMS.Interfaces;

public interface IUserSecurityService
{
    Task<DateTime?> GetLastPasswordChangeDateAsync(Guid userUuid);
}