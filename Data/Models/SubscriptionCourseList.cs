using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("subscriptions_courses_list", Schema = "public")]
public class SubscriptionCourseList
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; init; } = Guid.NewGuid();

    [Column("course_uuid")]
    public Guid CourseUuid { get; set; }

    [Column("subscription_uuid")]
    public Guid SubscriptionUuid { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [ForeignKey(nameof(SubscriptionUuid))]
    public virtual Subscription Subscription { get; set; } = null!;
    
    [ForeignKey(nameof(CourseUuid))]
     public virtual Course Course { get; set; } = null!;
}