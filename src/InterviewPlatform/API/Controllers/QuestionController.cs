using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;

namespace InterviewPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByCourseId(Guid courseId)
    {
        try
        {
            var questions = await _questionService.GetQuestionsByCourseIdAsync(courseId);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("interview/{interviewId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByInterviewId(Guid interviewId)
    {
        try
        {
            var questions = await _questionService.GetQuestionsByInterviewIdAsync(interviewId);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetQuestionById(Guid id)
    {
        try
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound();

            return Ok(question);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto dto)
    {
        try
        {
            var question = await _questionService.AddQuestionAsync(dto);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(Guid id, [FromBody] UpdateQuestionDto dto)
    {
        try
        {
            var question = await _questionService.UpdateQuestionAsync(id, dto);
            return Ok(question);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuestion(Guid id)
    {
        try
        {
            await _questionService.DeleteQuestionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
