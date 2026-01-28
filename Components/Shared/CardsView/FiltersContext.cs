namespace LMS.Components.Shared.CardsView;

public sealed class FiltersContext
{
    public Dictionary<string, string> DisplayNames { get; }
    public HashSet<string> RegisteredFilters { get; } = new();

    public FiltersContext(Dictionary<string, string> displayNames)
    {
        DisplayNames = displayNames;
    }

    public void Register(string filterKey)
    {
        RegisteredFilters.Add(filterKey);
    }

    public string GetDisplayName(string filterKey)
        => DisplayNames.TryGetValue(filterKey, out var name)
            ? name
            : filterKey;
}