using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _interviewService;
    private readonly IQuestionService _questionService;

    public InterviewsController(IInterviewService interviewService, IQuestionService questionService)
    {
        _interviewService = interviewService;
        _questionService = questionService;
    }

    [HttpGet("/api/courses/{courseId:guid}/interviews")]
    public async Task<IActionResult> GetByCourseId(Guid courseId)
    {
        var interviews = await _interviewService.GetInterviewsByCourseIdAsync(courseId);
        return Ok(interviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var interview = await _interviewService.GetInterviewByIdAsync(id);
        if (interview == null) return NotFound();
        return Ok(interview);
    }

    [HttpGet("{id:guid}/questions")]
    public async Task<IActionResult> GetQuestionsByInterviewId(Guid id)
    {
        var questions = await _questionService.GetQuestionsByInterviewIdAsync(id);
        return Ok(questions);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<IActionResult> Create([FromBody] CreateInterviewDto dto)
    {
        var interview = await _interviewService.CreateInterviewAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = interview.Id }, interview);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInterviewDto dto)
    {
        await _interviewService.UpdateInterviewAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _interviewService.DeleteInterviewAsync(id);
        return NoContent();
    }
}
