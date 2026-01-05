using Domain.Entities;

namespace Domain.Interfaces;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<bool> HasDuplicateOrderAsync(Guid courseId, int order, Guid? excludeLessonId = null);
    Task<Lesson?> GetAdjacentLessonAsync(Guid courseId, int currentOrder, bool isUp);
}
