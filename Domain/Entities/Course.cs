using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public void Publish()
    {
        if (!Lessons.Any(l => !l.IsDeleted))
            throw new BusinessException("Course must have at least one active lesson");

        Status = CourseStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        Status = CourseStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}