public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Lesson> Lessons { get; set; } = new();

    public void Publish()
    {
        if (!Lessons.Any(l => !l.IsDeleted))
            throw new BusinessException("Course must have at least one active lesson");

        Status = CourseStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}