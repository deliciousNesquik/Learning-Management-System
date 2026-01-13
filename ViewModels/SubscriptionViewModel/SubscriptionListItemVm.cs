namespace LMS.ViewModels.SubscriptionViewModel;

public class SubscriptionListItemVm
{
    public Guid Uuid { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int EmployeesLimit { get; init; }
    public bool IsActive { get; init; }
    public decimal? Price { get; init; }
    public string Currency { get; init; } = "RUB";
    public string BranchName { get; init; } = string.Empty;
    public Guid BranchUuid { get; init; }
    public int CoursesCount { get; init; } // Количество привязанных курсов
    public List<Guid>? Courses { get; set; }
    public DateTime CreatedAt { get; init; }
}