using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Data.Entities;

[Table("materials_courses_list", Schema = "public")]
public class MaterialCourse
{
    [Key, Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("material_uuid")]
    public Guid MaterialUuid { get; set; }

    [Column("course_uuid")]
    public Guid CourseUuid { get; set; }

    [ForeignKey(nameof(MaterialUuid))]
    public Material? Material { get; set; }

    [ForeignKey(nameof(CourseUuid))]
    public Course? Course { get; set; }
}