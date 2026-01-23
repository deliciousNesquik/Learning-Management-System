namespace LMS.Components.CardsView;

public class CardsState
{
    public int PageSize { get; set; } = 12;
    public int CurrentPage { get; set; } = 1;
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }
    public Dictionary<string, object?> Filters { get; set; } = new();
}