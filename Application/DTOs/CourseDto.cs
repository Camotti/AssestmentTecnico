using Domain.Enums;

namespace Application.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateCourseDto
{
    public string Title { get; set; } = string.Empty;
}

public class CourseSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    public int TotalLessons { get; set; }
    public DateTime LastModified { get; set; }
}
