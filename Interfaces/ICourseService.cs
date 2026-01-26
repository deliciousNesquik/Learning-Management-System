using LMS.Data.Entities;
using LMS.DTOs.Course;

namespace LMS.Interfaces;

public interface ICourseService
{
    Task<List<CourseListItemVm>> GetCoursesForCardsAsync();
    Task<Course?> GetCourseWithDetailsAsync(Guid courseUuid);
}