using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class LessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public LessonService(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        
        return lessons
            .OrderBy(l => l.Order)
            .Select(l => new LessonDto
            {
                Id = l.Id,
                CourseId = l.CourseId,
                Title = l.Title,
                Order = l.Order,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            });
    }

    public async Task<LessonDto> CreateLessonAsync(Guid courseId, CreateLessonDto dto)
    {
        // Validate course exists
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new BusinessException("Course not found");

        // Validate unique order
        var hasDuplicate = await _lessonRepository.HasDuplicateOrderAsync(courseId, dto.Order);
        if (hasDuplicate)
            throw new BusinessException($"A lesson with order {dto.Order} already exists in this course");

        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = dto.Title,
            Order = dto.Order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _lessonRepository.AddAsync(lesson);

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Order = lesson.Order,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }

    public async Task<LessonDto> UpdateLessonAsync(Guid id, UpdateLessonDto dto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new BusinessException("Lesson not found");

        // Validate unique order if changed
        if (lesson.Order != dto.Order)
        {
            var hasDuplicate = await _lessonRepository.HasDuplicateOrderAsync(
                lesson.CourseId, dto.Order, id);
            if (hasDuplicate)
                throw new BusinessException($"A lesson with order {dto.Order} already exists in this course");
        }

        lesson.Title = dto.Title;
        lesson.Order = dto.Order;
        lesson.UpdatedAt = DateTime.UtcNow;

        await _lessonRepository.UpdateAsync(lesson);

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Order = lesson.Order,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }

    public async Task DeleteLessonAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new BusinessException("Lesson not found");

        lesson.SoftDelete();
        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task ReorderLessonAsync(Guid id, string direction)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new BusinessException("Lesson not found");

        bool isUp = direction.ToLower() == "up";
        var adjacentLesson = await _lessonRepository.GetAdjacentLessonAsync(
            lesson.CourseId, lesson.Order, isUp);

        if (adjacentLesson == null)
            throw new BusinessException($"Cannot move lesson {direction}");

        // Swap orders
        var tempOrder = lesson.Order;
        lesson.Order = adjacentLesson.Order;
        adjacentLesson.Order = tempOrder;

        lesson.UpdatedAt = DateTime.UtcNow;
        adjacentLesson.UpdatedAt = DateTime.UtcNow;

        await _lessonRepository.UpdateAsync(lesson);
        await _lessonRepository.UpdateAsync(adjacentLesson);
    }
}
