using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("subscriptions", Schema = "public")]
public class Subscription
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; init; } = Guid.NewGuid();

    [Column("name")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("employees_limit")]
    public int EmployeesLimit { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("price")]
    public decimal? Price { get; set; }

    [Column("currency")]
    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "RUB";

    [Column("billing_period")]
    public TimeSpan? BillingPeriod { get; set; }

    [Column("branch_uuid")]
    public Guid BranchUuid { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; init; } = DateTime.Now;

    // Навигационные свойства
    [ForeignKey(nameof(BranchUuid))]
    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<SubscriptionCourseList> Courses { get; set; } = new List<SubscriptionCourseList>();
}