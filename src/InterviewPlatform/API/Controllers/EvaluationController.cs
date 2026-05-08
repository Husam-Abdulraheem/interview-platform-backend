using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;

namespace InterviewPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EvaluationController : ControllerBase
{
    private readonly IAiEvaluationService _evaluationService;

    public EvaluationController(IAiEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    [HttpPost]
    public async Task<ActionResult<AiEvaluationResultDto>> EvaluateAnswer([FromBody] EvaluationRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Question) || string.IsNullOrWhiteSpace(request.Answer))
            {
                return BadRequest(new { error = "Question and answer are required." });
            }

            var result = await _evaluationService.EvaluateDirectAsync(request);
            return Ok(ApiResponse<AiEvaluationResultDto>.SuccessResult(result, "Evaluation completed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
