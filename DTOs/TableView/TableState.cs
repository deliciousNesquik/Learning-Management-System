namespace LMS.DTOs.TableView;

public class TableState
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<string> VisibleColumns { get; set; } = [];
}