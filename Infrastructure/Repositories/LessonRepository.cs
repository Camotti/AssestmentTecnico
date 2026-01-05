using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task<IEnumerable<Lesson>> GetAllAsync()
    {
        return await _context.Lessons.ToListAsync();
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<bool> HasDuplicateOrderAsync(Guid courseId, int order, Guid? excludeLessonId = null)
    {
        var query = _context.Lessons
            .Where(l => l.CourseId == courseId && l.Order == order);

        if (excludeLessonId.HasValue)
        {
            query = query.Where(l => l.Id != excludeLessonId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Lesson?> GetAdjacentLessonAsync(Guid courseId, int currentOrder, bool isUp)
    {
        if (isUp)
        {
            // Get lesson with the highest order that is less than current order
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && l.Order < currentOrder)
                .OrderByDescending(l => l.Order)
                .FirstOrDefaultAsync();
        }
        else
        {
            // Get lesson with the lowest order that is greater than current order
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && l.Order > currentOrder)
                .OrderBy(l => l.Order)
                .FirstOrDefaultAsync();
        }
    }

    public async Task<Lesson> AddAsync(Lesson entity)
    {
        await _context.Lessons.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Lesson entity)
    {
        _context.Lessons.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lesson entity)
    {
        _context.Lessons.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
