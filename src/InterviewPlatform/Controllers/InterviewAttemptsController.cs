using System.Security.Claims;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trainee")]
public class InterviewAttemptsController : ControllerBase
{
    private readonly IInterviewAttemptService _attemptService;

    public InterviewAttemptsController(IInterviewAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartAttempt([FromQuery] Guid interviewId)
    {
        var traineeId = GetUserId();
        if (traineeId == Guid.Empty) return Unauthorized();

        var attempt = await _attemptService.StartAttemptAsync(traineeId, interviewId);
        return Ok(attempt);
    }

    [HttpPost("submit-answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerDto dto)
    {
        try
        {
            var answer = await _attemptService.SubmitAnswerAsync(dto);
            return Ok(answer);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{attemptId:guid}/complete")]
    public async Task<IActionResult> CompleteAttempt(Guid attemptId)
    {
        try
        {
            var result = await _attemptService.CompleteAttemptAsync(attemptId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{attemptId:guid}")]
    public async Task<IActionResult> GetAttemptDetails(Guid attemptId)
    {
        var attempt = await _attemptService.GetAttemptDetailsAsync(attemptId);
        if (attempt == null) return NotFound();

        // Enforce that a user can only view their own attempts
        if (attempt.TraineeId != GetUserId()) return Forbid();

        return Ok(attempt);
    }
}
