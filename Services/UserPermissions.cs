using LMS.Data;
using LMS.Data.Entities;
using LMS.DTOs;
using LMS.DTOs.General;
using LMS.Models;
using LMS.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class UserPermissions(IDbContextFactory<DatabaseContext> dbFactory)
{
    private readonly List<(string Name, string Display, bool IsAdminOnly)> _allTables =
    [
        // --- Системные и инфраструктурные таблицы ---
    (DbSchemaConstants.AuditHistory, "История изменения БД", true),
    (DbSchemaConstants.UserPermissions, "Права пользователей", true),
    (DbSchemaConstants.LegalForms, "Формы организаций", true),

    // --- Пользователи и роли ---
    (DbSchemaConstants.Administrators, "Администраторы", true),
    (DbSchemaConstants.Moderators, "Модераторы", true),
    (DbSchemaConstants.Directors, "Руководители", true),
    (DbSchemaConstants.Employees, "Сотрудники", true),

    // --- Структура организации ---
    (DbSchemaConstants.Organizations, "Организации", true),
    (DbSchemaConstants.Branches, "Филиалы", true),
    (DbSchemaConstants.BranchesDirectors, "Директора к филиалам", true),
    (DbSchemaConstants.BranchesModerators, "Модераторы к филиалам", true),
    (DbSchemaConstants.EmployeesGroups, "Группы сотрудников", false),
    (DbSchemaConstants.GroupMemberships, "Членство в группах", false),

    // --- Обучение (Курсы) ---
    (DbSchemaConstants.Courses, "Курсы", false),
    (DbSchemaConstants.CoursesActivities, "Типы активностей", true),
    (DbSchemaConstants.CoursesCategories, "Категории курсов", true),
    (DbSchemaConstants.CoursesStatuses, "Статусы курсов", true),
    (DbSchemaConstants.CoursesAssignments, "Назначение курсов", false),
    (DbSchemaConstants.CoursesEnrollments, "Прохождение курсов", false),

    // --- Материалы ---
    (DbSchemaConstants.Materials, "Материалы", false),
    (DbSchemaConstants.MaterialsTypes, "Типы материалов", true),
    (DbSchemaConstants.MaterialsCoursesList, "Материалы к курсам", true),

    // --- Тестирование (Assessments) ---
    (DbSchemaConstants.Assessments, "Тесты", false),
    (DbSchemaConstants.AssessmentsAttempts, "Попытки тестирований", false),
    (DbSchemaConstants.Questions, "Вопросы", true),
    (DbSchemaConstants.QuestionsTypes, "Типы вопросов", true),
    (DbSchemaConstants.Answers, "Ответы", true),
    (DbSchemaConstants.EmployeesAnswers, "Ответы сотрудников", false),

    // --- Подписки ---
    (DbSchemaConstants.Subscriptions, "Подписки", true),
    (DbSchemaConstants.SubscriptionsCoursesList, "Курсы для подписки", true)
    ];

    /// <summary>Возвращает список таблиц которыми может управлять конкретный пользователь</summary>
    /// <param name="isAdmin">Параметр регулирующий какие таблицы необходимо отдать</param>
    /// <returns> IReadOnlyList(имя таблицы БД, имя таблицы понятное пользователю, только для админа) </returns>
    private List<(string Name, string Display, bool IsAdminOnly)> GetTablesForUser(bool isAdmin)
    {
        return isAdmin ? _allTables : _allTables.Where(p => !p.IsAdminOnly).ToList();
    }

    public async Task<Dictionary<string, HashSet<SqlOperation>>> GetAllPermissions(Guid userUuid)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
    
        var permissions = await db.UserPermissions
            .Where(p => p.UserUuid == userUuid)
            .ToListAsync();

        // Группируем права по имени таблицы для быстрого поиска
        return permissions.ToDictionary(
            p => p.TableName,
            p => {
                var ops = new HashSet<SqlOperation>();
                if (p.CanSelect) ops.Add(SqlOperation.Select);
                if (p.CanInsert) ops.Add(SqlOperation.Insert);
                if (p.CanUpdate) ops.Add(SqlOperation.Update);
                if (p.CanDelete) ops.Add(SqlOperation.Delete);
                return ops;
            }
        );
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