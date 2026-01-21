using LMS.Data.Models;
using LMS.ViewModels.CourseViewModel;

namespace LMS.Interfaces;

public interface ICourseService
{
    Task<List<CourseListItemVm>> GetCoursesForCardsAsync();
    Task<Course?> GetCourseWithDetailsAsync(Guid courseUuid);
}