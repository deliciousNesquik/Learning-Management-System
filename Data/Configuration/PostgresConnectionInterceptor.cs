using System.Data.Common;
using LMS.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LMS.Data.Configuration;

/// <summary>
/// Перехватчик подключения к PostgreSQL, обеспечивающий установку идентификатора текущего пользователя
/// в рамках транзакции для целей аудита изменений в базе данных.
/// </summary>
/// <remarks>
/// При открытии подключения к PostgreSQL устанавливает переменную сессии <c>app.current_user_id</c>,
/// используя значение из <see cref="UserRequestContext.UserUuid"/>. Эта переменная используется
/// в PostgreSQL-триггерах для аудита (<c>fn_audit_track_changes</c>) для определения того, 
/// какой пользователь выполнил изменение данных.
/// 
/// Используется <c>SET LOCAL</c>, что гарантирует, что значение переменной будет автоматически
/// сброшено после завершения транзакции. Это предотвращает влияние одного пользователя на другого
/// в условиях многопользовательской среды Blazor Server Interactive.
/// 
/// В целях безопасности идентификатор пользователя (GUID) проверяется на наличие значения
/// перед установкой. Если <see cref="UserRequestContext.UserUuid"/> равен null, установка
/// переменной не производится.
/// </remarks>
/// <param name="requestContext">
/// Контекст текущего запроса, содержащий идентификатор пользователя (<see cref="UserRequestContext.UserUuid"/>).
/// </param>
/// <example>
/// Пример использования в конфигурации DbContext:
/// <code>
/// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
/// {
///     optionsBuilder.UseNpgsql(connectionString)
///                   .AddInterceptors(new PostgresConnectionInterceptor(requestContext));
/// }
/// </code>
/// </example>
public class PostgresConnectionInterceptor(UserRequestContext requestContext) : DbConnectionInterceptor
{
    /// <summary>
    /// Метод вызывается после успешного открытия подключения к базе данных.
    /// Устанавливает идентификатор текущего пользователя в сессии PostgreSQL.
    /// </summary>
    /// <param name="connection">Открытое подключение к базе данных.</param>
    /// <param name="eventData">Данные события открытия подключения.</param>
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        // Получаем идентификатор пользователя из контекста запроса
        var userId = requestContext.UserUuid;
        
        // Устанавливаем идентификатор пользователя в сессии PostgreSQL, если он существует
        if (userId.HasValue)
        {
            using var command = connection.CreateCommand();
            // SET LOCAL гарантирует, что переменная будет сброшена после завершения транзакции
            command.CommandText = $"SET LOCAL app.current_user_id = '{userId}';";
            command.ExecuteNonQuery();
        }
        
        // Вызываем базовую реализацию для продолжения обработки события
        base.ConnectionOpened(connection, eventData);
    }
    
    /// <summary>
    /// Асинхронная версия метода, вызываемого после открытия подключения.
    /// Устанавливает идентификатор текущего пользователя в сессии PostgreSQL.
    /// </summary>
    /// <param name="connection">Открытое подключение к базе данных.</param>
    /// <param name="eventData">Данные события открытия подключения.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        // Получаем идентификатор пользователя из контекста запроса
        var userId = requestContext.UserUuid;
        
        // Устанавливаем идентификатор пользователя в сессии PostgreSQL, если он существует
        if (userId.HasValue)
        {
            await using var command = connection.CreateCommand();
            // SET LOCAL гарантирует, что переменная будет сброшена после завершения транзакции
            command.CommandText = $"SET LOCAL app.current_user_id = '{userId}';";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        
        // Вызываем базовую реализацию для продолжения обработки события
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}