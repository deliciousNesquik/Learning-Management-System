using LMS.Data.Entities;
using LMS.DTOs.CardsView;
using LMS.DTOs.Course;

namespace LMS.Interfaces;

public interface ICourseService
{
    Task<List<CourseListItemVm>> GetCoursesForCardsAsync(CardQuery query);
    Task<Course?> GetCourseWithDetailsAsync(Guid courseUuid);
}