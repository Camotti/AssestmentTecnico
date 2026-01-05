using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _courseService = new CourseService(_mockCourseRepository.Object);
    }

    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = Guid.NewGuid(),
                    CourseId = courseId,
                    Title = "Lesson 1",
                    Order = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            }
        };

        _mockCourseRepository
            .Setup(r => r.GetByIdWithLessonsAsync(courseId))
            .ReturnsAsync(course);

        _mockCourseRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);

        // Act
        await _courseService.PublishCourseAsync(courseId);

        // Assert
        course.Status.Should().Be(CourseStatus.Published);
        _mockCourseRepository.Verify(r => r.UpdateAsync(course), Times.Once);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Lessons = new List<Lesson>() // No lessons
        };

        _mockCourseRepository
            .Setup(r => r.GetByIdWithLessonsAsync(courseId))
            .ReturnsAsync(course);

        // Act & Assert
        var act = async () => await _courseService.PublishCourseAsync(courseId);
        
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Course must have at least one active lesson");

        _mockCourseRepository.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCourseRepository
            .Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        _mockCourseRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);

        // Act
        await _courseService.DeleteCourseAsync(courseId);

        // Assert
        course.IsDeleted.Should().BeTrue();
        _mockCourseRepository.Verify(r => r.UpdateAsync(course), Times.Once);
        _mockCourseRepository.Verify(r => r.DeleteAsync(It.IsAny<Course>()), Times.Never);
    }
}
