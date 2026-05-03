namespace InterviewPlatform.Application.DTOs;

public class AiEvaluationResultDto
{
    public decimal Score { get; set; }
    public List<string> Strengths { get; set; } = new List<string>();
    public List<string> Weaknesses { get; set; } = new List<string>();
    public List<string> Suggestions { get; set; } = new List<string>();
}
