using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        _mockLessonRepository = new Mock<ILessonRepository>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _lessonService = new LessonService(_mockLessonRepository.Object, _mockCourseRepository.Object);
    }

    [Fact]
    public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
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

        var createDto = new CreateLessonDto
        {
            Title = "New Lesson",
            Order = 1
        };

        _mockCourseRepository
            .Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        _mockLessonRepository
            .Setup(r => r.HasDuplicateOrderAsync(courseId, createDto.Order, null))
            .ReturnsAsync(false); // No duplicate

        _mockLessonRepository
            .Setup(r => r.AddAsync(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);

        // Act
        var result = await _lessonService.CreateLessonAsync(courseId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createDto.Title);
        result.Order.Should().Be(createDto.Order);
        _mockLessonRepository.Verify(r => r.AddAsync(It.IsAny<Lesson>()), Times.Once);
    }

    [Fact]
    public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
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

        var createDto = new CreateLessonDto
        {
            Title = "New Lesson",
            Order = 1
        };

        _mockCourseRepository
            .Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        _mockLessonRepository
            .Setup(r => r.HasDuplicateOrderAsync(courseId, createDto.Order, null))
            .ReturnsAsync(true); // Duplicate exists

        // Act & Assert
        var act = async () => await _lessonService.CreateLessonAsync(courseId, createDto);
        
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage($"A lesson with order {createDto.Order} already exists in this course");

        _mockLessonRepository.Verify(r => r.AddAsync(It.IsAny<Lesson>()), Times.Never);
    }
}
