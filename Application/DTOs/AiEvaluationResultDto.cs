namespace InterviewPlatform.Application.DTOs;

public class AiEvaluationResultDto
{
    public decimal Score { get; set; }
    public string Strengths { get; set; } = string.Empty;
    public string Weaknesses { get; set; } = string.Empty;
}
