namespace LMS.DTOs.CardsView;

/// <summary>
/// Описывает, что пользователь сейчас хочет видеть.
/// </summary>
public sealed class CardQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 6;

    public string? Search { get; set; }

    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }
    
    public Dictionary<string, object?> Filters { get; set; } = new();
}