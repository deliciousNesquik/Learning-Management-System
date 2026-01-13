using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using LMS.Services;

public class PostgresConnectionInterceptor : DbConnectionInterceptor
{
    private readonly UserSessionAccessor _sessionAccessor;

    public PostgresConnectionInterceptor(UserSessionAccessor sessionAccessor)
    {
        _sessionAccessor = sessionAccessor;
    }

    // Используем синхронный метод для стабильности
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        var userId = _sessionAccessor.UserUuid;
        if (userId.HasValue)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SET app.current_user_id = '{userId}';";
            command.ExecuteNonQuery();
        }
        base.ConnectionOpened(connection, eventData);
    }

    // И асинхронный тоже, для полноты картины
    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        var userId = _sessionAccessor.UserUuid;
        if (userId.HasValue)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SET app.current_user_id = '{userId}';";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}