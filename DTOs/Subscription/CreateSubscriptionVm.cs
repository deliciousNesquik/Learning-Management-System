using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Subscription;

public class CreateSubscriptionVm
{
    [Required(ErrorMessage = "Название подписки обязательно")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите филиал")]
    public Guid BranchUuid { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "Укажите дату окончания")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

    [Range(1, 1000000, ErrorMessage = "Лимит должен быть больше 0")]
    public int EmployeesLimit { get; set; }

    public decimal Price { get; set; }
    
    [StringLength(3)]
    public string Currency { get; set; } = "RUB";

    public bool IsActive { get; set; } = true;

    // Интервал (например, для логики автопродления)
    public TimeSpan? BillingPeriod { get; set; }

    // Список UUID выбранных курсов для этой подписки
    public List<Guid> SelectedCourseUuids { get; set; } = new();
}