namespace LMS.Models;

public static class DbSchemaConstants
{
    // --- Системные и инфраструктурные таблицы ---
    public const string AuditHistory = "audit_history";
    public const string UserPermissions = "user_permissions";
    public const string LegalForms = "legal_forms";

    // --- Пользователи и роли ---
    public const string Administrators = "administrators";
    public const string Moderators = "moderators";
    public const string Directors = "directors";
    public const string Employees = "employees";

    // --- Структура организации ---
    public const string Organizations = "organizations";
    public const string Branches = "branches";
    public const string BranchesDirectors = "branches_directors";
    public const string BranchesModerators = "branches_moderators";
    public const string EmployeesGroups = "employees_groups";
    public const string GroupMemberships = "group_memberships";

    // --- Обучение (Курсы) ---
    public const string Courses = "courses";
    public const string CoursesActivities = "courses_activities";
    public const string CoursesCategories = "courses_categories";
    public const string CoursesStatuses = "courses_statuses";
    public const string CoursesAssignments = "courses_assignments";
    public const string CoursesEnrollments = "courses_enrollments";

    // --- Материалы ---
    public const string Materials = "materials";
    public const string MaterialsTypes = "materials_types";
    public const string MaterialsCoursesList = "materials_courses_list";

    // --- Тестирование (Assessments) ---
    public const string Assessments = "assessments";
    public const string AssessmentsAttempts = "assessments_attempts";
    public const string Questions = "questions";
    public const string QuestionsTypes = "questions_types";
    public const string Answers = "answers";
    public const string EmployeesAnswers = "employees_answers";

    // --- Подписки ---
    public const string Subscriptions = "subscriptions";
    public const string SubscriptionsCoursesList = "subscriptions_courses_list";
}