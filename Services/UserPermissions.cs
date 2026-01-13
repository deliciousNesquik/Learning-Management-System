using LMS.Data;
using LMS.Data.Models;
using LMS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class UserPermissions(IDbContextFactory<DatabaseContext> dbFactory)
{
    private readonly List<(string Name, string Display)> _allTables =
    [
        ("audit_history", "История изменения БД"),
        ("administrators", "Администраторы"),
        ("legal_forms", "Формы организаций"),
        ("organizations", "Организации"),
        ("branches", "Филиалы"),
        ("branches_directors", "Директора к филиалам"),
        ("directors", "Руководители организаций"),
        ("branches_moderators", "Модераторы к филиалам"),
        ("subscriptions", "Подписки"),
        ("subscriptions_courses_list", "Курсы для подписки"),
        ("courses_activities", "Типы активностей"),
        ("courses_categories", "Категории курсов"),
        ("courses_statuses", "Типы статусов"),
        ("materials_types", "Типы материалов"),
        ("questions_types", "Типы вопросов"),
        ("answers", "Ответы"),
        ("assessments", "Тесты"),
        ("materials", "Материалы"),
        ("materials_courses_list", "Материалы к курсам"),
        ("questions", "Вопросы"),

        // --- Модераторские таблицы ---
        ("moderators", "Модераторы"),
        ("assessments_attempts", "Попытки тестирований"),
        ("courses", "Курсы"),
        ("courses_assignments", "Назначение курсов"),
        ("courses_enrollments", "Прохождение курсов"),
        ("employees", "Сотрудники"),
        ("employees_answers", "Ответы сотрудников"),
        ("employees_groups", "Группы сотрудников"),
        ("group_memberships", "Отношение сотрудника к группе")
    ];

    private IReadOnlyList<(string Name, string Display)> GetTablesForUser(bool isAdmin)
    {
        if (isAdmin)
        {
            // админ видит все таблицы
            return _allTables;
        }
    
        // модератор видит только нижний блок
        var moderatorTablesStart = _allTables.FindIndex(t => t.Name == "moderators");
        if (moderatorTablesStart < 0)
            return Array.Empty<(string, string)>();

        return _allTables.Skip(moderatorTablesStart).ToList();
    }

    public async Task<bool> HasPermission(Guid? userUuid, string tableName, string action)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
    
        var p = await db.UserPermissions
            .FirstOrDefaultAsync(x => x.UserUuid == userUuid && x.TableName == tableName);
        
        if (p == null) return false;

        return action.ToLower() switch
        {
            "select" => p.CanSelect,
            "insert" => p.CanInsert,
            "update" => p.CanUpdate,
            "delete" => p.CanDelete,
            _ => false
        };
    }
    
    public async Task<List<TablePermissionVm>> GetPermissionsAsync(Guid userUuid, bool isAdmin = false)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // Загружаем те права, которые уже есть в базе
        var dbPermissions = await db.UserPermissions
            .Where(p => p.UserUuid == userUuid)
            .ToListAsync();

        // Формируем полный список для UI (26 строк)
        var result = new List<TablePermissionVm>();
        
        var tablesForUi = GetTablesForUser(isAdmin);

        foreach (var table in tablesForUi)
        {
            var existing = dbPermissions.FirstOrDefault(p => p.TableName == table.Name);
            result.Add(new TablePermissionVm
            {
                TableName = table.Name,
                DisplayName = table.Display,
                CanSelect = existing?.CanSelect ?? false,
                CanInsert = existing?.CanInsert ?? false,
                CanUpdate = existing?.CanUpdate ?? false,
                CanDelete = existing?.CanDelete ?? false
            });
        }

        return result;
    }

    public async Task UpdatePermissionsAsync(Guid userUuid, List<TablePermissionVm> permissions)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // 1. Удаляем старые права этого пользователя (самый простой способ обновления)
        var oldPermissions = await db.UserPermissions.Where(p => p.UserUuid == userUuid).ToListAsync();
        db.UserPermissions.RemoveRange(oldPermissions);

        // 2. Добавляем новые только те, где выбрана хотя бы одна галочка
        // (чтобы не забивать БД пустыми строками)
        var newPermissions = permissions
            .Where(p => p.CanSelect || p.CanInsert || p.CanUpdate || p.CanDelete)
            .Select(p => new UserPermission
            {
                UserUuid = userUuid,
                TableName = p.TableName,
                DisplayName = p.DisplayName,
                CanSelect = p.CanSelect,
                CanInsert = p.CanInsert,
                CanUpdate = p.CanUpdate,
                CanDelete = p.CanDelete
            });

        await db.UserPermissions.AddRangeAsync(newPermissions);
        await db.SaveChangesAsync();
    }
}