using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("branches", Schema = "public")]
public class Branch
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; } = Guid.NewGuid();
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("is_default")]
    public bool IsDefault { get; set; }
    
    [Column("branch_code")]
    public string BranchCode { get; set; }
    
    [Column("status")]
    public bool Status { get; set; }
    
    [Column("region")]
    public string Region { get; set; }
    
    [Column("city")]
    public string City { get; set; }
    
    [Column("street")]
    public string Street { get; set; }
    
    [Column("house_number")]
    public string HouseNumber { get; set; }
    
    [Column("organization_uuid")]
    public Guid OrganizationUuid { get; set; }
    
    [Column("timezone")]
    public int Timezone { get; set; }

    [Column("created_at")] 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(OrganizationUuid))]
    public virtual Organization Organization { get; set; } = null!;
}