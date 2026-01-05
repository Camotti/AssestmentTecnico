using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<(IEnumerable<Course> Courses, int TotalCount)> SearchAsync(
        string? searchQuery, 
        CourseStatus? status, 
        int page, 
        int pageSize);
    
    Task<Course?> GetByIdWithLessonsAsync(Guid id);
}
