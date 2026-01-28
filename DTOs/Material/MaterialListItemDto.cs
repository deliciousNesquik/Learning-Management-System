namespace LMS.DTOs.Material;

public class MaterialListItemDto
{
    public Guid Uuid { get; set; }
    public string Title { get; set; }
    public string TypeName { get; set; }
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; }
    
    public long FileSizeBytes { get; set; } 
    public DateTime CreatedAt { get; set; }
    
    public string? S3Key { get; set; }
    public string? AccessUrl { get; set; }
}