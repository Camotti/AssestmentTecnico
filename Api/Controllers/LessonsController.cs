using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly LessonService _lessonService;

    public LessonsController(LessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetByCourse(Guid courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseAsync(courseId);
        return Ok(lessons);
    }

    [HttpPost("courses/{courseId}/lessons")]
    public async Task<ActionResult<LessonDto>> Create(Guid courseId, [FromBody] CreateLessonDto dto)
    {
        var lesson = await _lessonService.CreateLessonAsync(courseId, dto);
        return CreatedAtAction(nameof(GetByCourse), new { courseId }, lesson);
    }

    [HttpPut("lessons/{id}")]
    public async Task<ActionResult<LessonDto>> Update(Guid id, [FromBody] UpdateLessonDto dto)
    {
        var lesson = await _lessonService.UpdateLessonAsync(id, dto);
        return Ok(lesson);
    }

    [HttpDelete("lessons/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _lessonService.DeleteLessonAsync(id);
        return NoContent();
    }

    [HttpPatch("lessons/{id}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderLessonDto dto)
    {
        await _lessonService.ReorderLessonAsync(id, dto.Direction);
        return NoContent();
    }
}
