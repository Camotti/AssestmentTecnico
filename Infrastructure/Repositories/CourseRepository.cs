using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task<Course?> GetByIdWithLessonsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<(IEnumerable<Course> Courses, int TotalCount)> SearchAsync(
        string? searchQuery, 
        CourseStatus? status, 
        int page, 
        int pageSize)
    {
        var query = _context.Courses.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(c => c.Title.Contains(searchQuery));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var courses = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (courses, totalCount);
    }

    public async Task<Course> AddAsync(Course entity)
    {
        await _context.Courses.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Course entity)
    {
        _context.Courses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course entity)
    {
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
