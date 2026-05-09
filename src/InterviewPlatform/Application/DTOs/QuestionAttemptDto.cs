namespace InterviewPlatform.Application.DTOs;

public class QuestionAttemptDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string TraineeAnswer { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string GeneralFeedback { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = new List<string>();
    public List<string> Weaknesses { get; set; } = new List<string>();
    public List<string> Suggestions { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
    
    public QuestionDto? Question { get; set; }
}
