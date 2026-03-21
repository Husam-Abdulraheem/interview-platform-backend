using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _interviewService;

    public InterviewsController(IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    [HttpGet("course/{courseId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCourseId(Guid courseId)
    {
        var interviews = await _interviewService.GetInterviewsByCourseIdAsync(courseId);
        return Ok(interviews);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var interview = await _interviewService.GetInterviewByIdAsync(id);
        if (interview == null) return NotFound();
        return Ok(interview);
    }

    [HttpPost]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInterviewDto dto)
    {
        var interview = await _interviewService.CreateInterviewAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = interview.Id }, interview);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInterviewDto dto)
    {
        await _interviewService.UpdateInterviewAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _interviewService.DeleteInterviewAsync(id);
        return NoContent();
    }
}
