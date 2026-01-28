using LMS.Data.Entities;
using LMS.DTOs.CardsView;
using LMS.DTOs.Course;
using LMS.DTOs.TableView;

namespace LMS.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseListItemVm>> GetCoursesForCardsAsync(CardQuery query);
    Task<Course?> GetCourseWithDetailsAsync(Guid courseUuid);
}