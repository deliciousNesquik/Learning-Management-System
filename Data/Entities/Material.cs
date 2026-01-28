using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("materials", Schema = "public")]
public class Material
{
    [Key, Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("bucket_name")] // Здесь мы храним Key файла в S3
    public required string S3Key { get; set; }

    [Column("type_uuid")]
    public Guid TypeUuid { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("size_bytes")]
    public long? SizeBytes { get; set; }

    [ForeignKey(nameof(TypeUuid))]
    public MaterialType? Type { get; set; }

    public ICollection<MaterialCourse> CourseLinks { get; set; } = new List<MaterialCourse>();
}