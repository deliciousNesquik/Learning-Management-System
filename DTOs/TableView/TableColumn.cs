using Microsoft.AspNetCore.Components;

namespace LMS.DTOs.TableView;

/// <summary>
/// Описывает одну колонку таблицы.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public sealed class TableColumn<TItem>
{
    public string Title { get; init; } = "";
    
    // Как получить значение из строки
    public Func<TItem, object?> Value { get; init; } = _ => null;

    // Можно ли сортировать
    public bool Sortable { get; init; }

    // Ключ сортировки (используется сервисом)
    public string? SortKey { get; init; }
    
    // Шаблон для сложной разметки (кнопки, значки, ссылки)
    // Здесь должен быть именно TItem, а не T
    public RenderFragment<TItem>? Template { get; set; }
}