using Application.DTOs;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly CourseService _courseService;

    public CoursesController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResultDto<CourseDto>>> Search(
        [FromQuery] string? q,
        [FromQuery] CourseStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _courseService.SearchCoursesAsync(q, status, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}/summary")]
    public async Task<ActionResult<CourseSummaryDto>> GetSummary(Guid id)
    {
        var summary = await _courseService.GetCourseSummaryAsync(id);
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
    {
        var course = await _courseService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetSummary), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> Update(Guid id, [FromBody] UpdateCourseDto dto)
    {
        var course = await _courseService.UpdateCourseAsync(id, dto);
        return Ok(course);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _courseService.DeleteCourseAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        await _courseService.PublishCourseAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        await _courseService.UnpublishCourseAsync(id);
        return NoContent();
    }
}
