using LMS.DTOs.Subscription;
using LMS.DTOs.TableView;
using LMS.Models.UserManipulationResult;

namespace LMS.Interfaces;

public interface ISubscriptionService
{
    /// <summary>
    /// Получение постраничного списка подписок для таблицы
    /// </summary>
    Task<PagedResult<SubscriptionListItemVm>> GetPagedAsync(TableQuery query);

    /// <summary>
    /// Создание новой подписки с привязкой курсов
    /// </summary>
    Task<CreateUserResult> CreateAsync(CreateSubscriptionVm model);

    /// <summary>
    /// Обновление данных подписки и синхронизация списка курсов
    /// </summary>
    Task<UpdateUserResult> UpdateAsync(EditSubscriptionVm model);

    /// <summary>
    /// Удаление подписки и связанных с ней записей о курсах
    /// </summary>
    Task<DeleteUserResult> DeleteAsync(Guid uuid);

    /// <summary>
    /// Получение списка всех доступных курсов для выбора в выпадающем списке
    /// </summary>
    Task<List<CourseLookupVm>> GetAllCoursesForLookupAsync();

    /// <summary>
    /// Список доступных курсов для филиала
    /// </summary>
    /// <param name="branchUuid">Идентификатор филиала</param>
    /// <returns></returns>
    Task<List<Guid>> GetAllAvailableCourses(Guid branchUuid);
}