using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewAttemptsController : ControllerBase
{
    private readonly IInterviewAttemptService _attemptService;

    public InterviewAttemptsController(IInterviewAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    [HttpPost("start")]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> StartAttempt([FromQuery] Guid traineeId, [FromQuery] Guid interviewId)
    {
        var attempt = await _attemptService.StartAttemptAsync(traineeId, interviewId);
        return Ok(attempt);
    }

    [HttpPost("submit-answer")]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerDto dto)
    {
        var result = await _attemptService.SubmitAnswerAsync(dto);
        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> CompleteAttempt(Guid id)
    {
        var attempt = await _attemptService.CompleteAttemptAsync(id);
        return Ok(attempt);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttemptDetails(Guid id)
    {
        var details = await _attemptService.GetAttemptDetailsAsync(id);
        if (details == null) return NotFound();
        return Ok(details);
    }
}
