using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface IAiEvaluationService
{
    Task<AiEvaluationResultDto> EvaluateAnswerAsync(string questionContent, string traineeAnswer);
}
