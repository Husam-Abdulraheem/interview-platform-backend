using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<IActionResult> AddQuestion([FromBody] CreateQuestionDto dto)
    {
        var question = await _questionService.AddQuestionAsync(dto);
        // We do not have a GetQuestionById so we just return Ok with the created object
        return Ok(question);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _questionService.DeleteQuestionAsync(id);
        return NoContent();
    }
}
