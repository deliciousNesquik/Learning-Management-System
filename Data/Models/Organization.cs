using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Models;

[Table("organizations", Schema = "public")]
public sealed class Organization
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("mail")]
    public string? Mail { get; set; }
    
    [Column("contacts")]
    public string? Contacts { get; set; }

    [Column("taxpayer")]
    public long Taxpayer { get; set; } // bigint -> long

    [Column("legal_form_uuid")]
    public Guid LegalFormUuid { get; set; }
    
    [Column("license_number")]
    public string? LicenseNumber { get; set; }
    
    [Column("license_valid_from")]
    public DateTime LicenseValidFrom { get; set; }

    [Column("license_valid_to")]
    public DateTime LicenseValidTo { get; set; }

    [Column("accreditation_info")]
    public string? AccreditationInfo { get; set; }

    [Column("timezone")]
    public int Timezone { get; set; }
    
    [Column("region")]
    public string? Region { get; set; }
    
    [Column("city")]
    public string? City { get; set; }
    
    [Column("street")]
    public string? Street { get; set; }
    
    [Column("house_number")]
    public string? HouseNumber { get; set; }
    
    [ForeignKey(nameof(LegalFormUuid))]
    public LegalForm LegalForm { get; set; } = null!;
}