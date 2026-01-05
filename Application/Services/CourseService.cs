using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class CourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<PagedResultDto<CourseDto>> SearchCoursesAsync(
        string? searchQuery, 
        CourseStatus? status, 
        int page = 1, 
        int pageSize = 10)
    {
        var (courses, totalCount) = await _courseRepository.SearchAsync(searchQuery, status, page, pageSize);

        return new PagedResultDto<CourseDto>
        {
            Items = courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<CourseSummaryDto> GetCourseSummaryAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(id);
        if (course == null)
            throw new BusinessException("Course not found");

        return new CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status,
            TotalLessons = course.Lessons.Count(l => !l.IsDeleted),
            LastModified = course.UpdatedAt
        };
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _courseRepository.AddAsync(course);
        
        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public async Task<CourseDto> UpdateCourseAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new BusinessException("Course not found");

        course.Title = dto.Title;
        course.UpdatedAt = DateTime.UtcNow;

        await _courseRepository.UpdateAsync(course);

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new BusinessException("Course not found");

        course.SoftDelete();
        await _courseRepository.UpdateAsync(course);
    }

    public async Task PublishCourseAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(id);
        if (course == null)
            throw new BusinessException("Course not found");

        course.Publish(); // This validates business rules
        await _courseRepository.UpdateAsync(course);
    }

    public async Task UnpublishCourseAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new BusinessException("Course not found");

        course.Unpublish();
        await _courseRepository.UpdateAsync(course);
    }
}
