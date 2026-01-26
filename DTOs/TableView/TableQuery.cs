namespace LMS.DTOs.TableView;

/// <summary>
/// Описывает, что пользователь сейчас хочет видеть.
/// </summary>
public sealed class TableQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }
    
    public Dictionary<string, object?> Filters { get; set; } = new();
}