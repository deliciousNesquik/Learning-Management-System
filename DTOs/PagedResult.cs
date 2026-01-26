namespace LMS.DTOs.TableView;

/// <summary>
/// Контейнер данных для таблицы.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}