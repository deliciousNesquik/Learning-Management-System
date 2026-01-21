namespace LMS.ViewModels.CourseViewModel;

public class CourseListItemVm
{
    public Guid Uuid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#828282";
    public string? ShortDescription { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MaterialCount { get; set; }
    public int AssessmentCount { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}