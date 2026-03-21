namespace InterviewPlatform.Core.Entities;

public class AnswerAttempt
{
    public Guid Id { get; set; }
    public Guid InterviewAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string SubmittedText { get; set; } = string.Empty;
    public decimal AiScore { get; set; }
    public string AiStrengths { get; set; } = string.Empty;
    public string AiWeaknesses { get; set; } = string.Empty;

    public InterviewAttempt InterviewAttempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
