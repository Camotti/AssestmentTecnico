using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Order)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .IsRequired();

        // Unique constraint on CourseId + Order for non-deleted lessons
        // Note: EF Core doesn't support filtered unique indexes directly in fluent API
        // We'll create this in the migration manually if needed, or handle in repository
        builder.HasIndex(l => new { l.CourseId, l.Order })
            .IsUnique();

        builder.HasIndex(l => l.IsDeleted);
    }
}
