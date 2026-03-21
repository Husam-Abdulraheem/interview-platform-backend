using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("interview/{interviewId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInterviewId(Guid interviewId)
    {
        var questions = await _questionService.GetQuestionsByInterviewIdAsync(interviewId);
        return Ok(questions);
    }

    [HttpPost]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
    {
        var question = await _questionService.AddQuestionAsync(dto);
        return Created("", question); 
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _questionService.DeleteQuestionAsync(id);
        return NoContent();
    }
}
