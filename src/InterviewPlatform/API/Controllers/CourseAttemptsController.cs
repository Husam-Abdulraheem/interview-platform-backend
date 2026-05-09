using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;

namespace InterviewPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseAttemptsController : ControllerBase
{
    private readonly ICourseAttemptService _attemptService;

    public CourseAttemptsController(ICourseAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<CourseAttemptDto>>> StartAttempt([FromBody] StartCourseAttemptDto request)
    {
        var attempt = await _attemptService.StartAttemptAsync(request);
        return Ok(ApiResponse<CourseAttemptDto>.SuccessResult(attempt, "Attempt started successfully"));
    }

    [HttpPost("{id:guid}/submit-answer")]
    public async Task<ActionResult<ApiResponse<QuestionAttemptDto>>> SubmitAnswer(Guid id, [FromBody] SubmitAnswerDto request)
    {
        var result = await _attemptService.SubmitAnswerAsync(id, request);
        return Ok(ApiResponse<QuestionAttemptDto>.SuccessResult(result, "Answer submitted and evaluated successfully"));
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<ApiResponse<CourseAttemptDto>>> CompleteAttempt(Guid id)
    {
        var attempt = await _attemptService.CompleteAttemptAsync(id);
        return Ok(ApiResponse<CourseAttemptDto>.SuccessResult(attempt, "Attempt completed successfully"));
    }

    [HttpGet("my-attempts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CourseAttemptDto>>>> GetMyAttempts()
    {
        var attempts = await _attemptService.GetMyAttemptsAsync();
        return Ok(ApiResponse<IEnumerable<CourseAttemptDto>>.SuccessResult(attempts, "Attempts retrieved successfully"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CourseAttemptDto>>> GetAttemptById(Guid id)
    {
        var attempt = await _attemptService.GetAttemptByIdAsync(id);
        if (attempt == null)
            return NotFound(ApiResponse<object>.Fail("Attempt not found"));

        return Ok(ApiResponse<CourseAttemptDto>.SuccessResult(attempt, "Attempt retrieved successfully"));
    }
}
